using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xabbo.Core.Serialization;

namespace Xabbo.Core.GameData;

public class GameDataLoader(
    string? cachePath = null,
    IGameDataUrlProvider? urlProvider = null,
    ILoggerFactory? loggerFactory = null
)
    : IGameDataLoader
{
    private readonly string _cachePath = cachePath
        ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "xabbo", "gamedata");
    private readonly IGameDataUrlProvider _urlProvider = urlProvider ?? new GameDataUrlProvider();
    private readonly ILogger _logger = (ILogger?)loggerFactory?.CreateLogger<GameDataLoader>() ?? NullLogger.Instance;
    private readonly HttpClient _http = new(new HttpClientHandler { AllowAutoRedirect = false })
    {
        DefaultRequestHeaders = {
            { "User-Agent", "xabbo" }
        }
    };

    public bool CacheOnly { get; set; }
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromHours(4);

    private static GameDataType? GetGameDataTypeFromName(string name) => name switch
    {
        "figurepartlist" => GameDataType.FigureData,
        "furnidata" => GameDataType.FurniData,
        "productdata" => GameDataType.ProductData,
        "external_texts" => GameDataType.ExternalTexts,
        "external_variables" => GameDataType.ExternalVariables,
        _ => null
    };

    private static string GetSimpleNameForGameDataType(GameDataType type)
    {
        string name = type.ToString().ToLower();
        if (name.StartsWith("external"))
            name = name[8..];
        if (name.EndsWith("data"))
            name = name[..^4];
        return name;
    }

    private string GetHashesFilePath(Hotel hotel) => Path.Join(_cachePath, hotel.Identifier, "hashes.json");

    private static async Task<GameDataHashes?> LoadHashesFromFileAsync(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        return await JsonSerializer.DeserializeAsync<GameDataHashes>(stream, XabboJsonContext.Default.GameDataHashes);
    }

    public async Task UpdateHashesAsync(Hotel hotel, GameDataHashes hashes)
    {
        FileInfo fileInfo = new FileInfo(GetHashesFilePath(hotel));
        fileInfo.Directory?.Create();

        using var stream = fileInfo.Open(FileMode.Create);
        await JsonSerializer.SerializeAsync(stream, hashes, XabboJsonContext.Default.GameDataHashes);

    }

    private static bool TryGetHashFromETag(HttpResponseMessage response, [NotNullWhen(true)] out string? hash)
    {
        if (response.Headers.TryGetValues("etag", out var values))
        {
            string? etag = values.FirstOrDefault();
            if (etag is not null && etag.StartsWith('"') && etag.EndsWith('"'))
            {
                hash = etag[1..^1];
                return true;
            }
        }

        hash = null;
        return false;
    }

    private static bool TryGetHashFromLocation(HttpResponseMessage response, [NotNullWhen(true)] out string? hash)
    {
        if (response.Headers.Location?.PathAndQuery.Split('/') is [ .., string { Length: 32 or 40 } segment ] &&
            segment.All(char.IsAsciiHexDigit))
        {
            hash = segment;
            return true;
        }

        hash = null;
        return false;
    }


    private async Task<string> FetchHashAsync(Hotel hotel, GameDataType type, CancellationToken cancellationToken)
    {
        string? url = _urlProvider.GetGameDataUrl(hotel, type);
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new Exception($"No game data URL for {type} on {hotel.Name} hotel.");
        }

        _logger.LogInformation("Fetching hash for {GameDataType}...", type);

        var res = await _http.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Head,
            RequestUri = new Uri(url)
        }, cancellationToken);

        switch (res.StatusCode)
        {
            case HttpStatusCode.OK:
                if (TryGetHashFromETag(res, out string? hash))
                    return hash;
                throw new Exception($"Failed to get hash from ETag for {type} on {hotel} hotel.");
            case HttpStatusCode.TemporaryRedirect:
                throw new Exception($"Failed to get hash from Location header for {type} on {hotel} hotel.");
            default:
                throw new Exception(
                    $"Server responded {(int)res.StatusCode} {res.ReasonPhrase} when attempting to"
                    + $" fetch hash for {type} on {hotel} hotel.");
        }
    }

    public async Task<GameDataHashes> LoadHashesAsync(Hotel hotel, CancellationToken cancellationToken = default)
    {
        string hashesFilePath = Path.Join(_cachePath, hotel.Identifier, "hashes.json");

        GameDataHashes? cachedHashes = null;
        GameDataHashes? fetchedHashes = null;

        var hashesFile = new FileInfo(hashesFilePath);
        if (hashesFile.Exists)
        {
            var hashesFileAge = DateTime.Now - hashesFile.LastWriteTime;
            _logger.LogDebug("Found cached hashes file at '{FilePath}'. (age: {Age})",
                hashesFile.FullName, hashesFileAge);

            try
            {
                cachedHashes = await LoadHashesFromFileAsync(hashesFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize hashes file.");
            }

            if (cachedHashes is not null && hashesFileAge < MaxAge)
            {
                _logger.LogInformation("Using cached hashes.");
                return cachedHashes;
            }
        }

        string? hashesUrl = _urlProvider.GetHashesUrl(hotel);

        if (hashesUrl is null)
        {
            if (hotel.IsOrigins)
            {
                var fetchFigureDataHashTask = FetchHashAsync(hotel, GameDataType.FigureData, cancellationToken);
                var fetchFurniDataHashTask = FetchHashAsync(hotel, GameDataType.FurniData, cancellationToken);
                var fetchExternalTextsHashTask = FetchHashAsync(hotel, GameDataType.ExternalTexts, cancellationToken);

                await Task.WhenAll(
                    fetchFigureDataHashTask,
                    fetchFurniDataHashTask,
                    fetchExternalTextsHashTask
                );

                fetchedHashes = new GameDataHashes
                {
                    [GameDataType.FigureData] = await fetchFigureDataHashTask,
                    [GameDataType.FurniData] = await fetchFurniDataHashTask,
                    [GameDataType.ExternalTexts] = await fetchExternalTextsHashTask,
                };
            }
            else
            {
                throw new Exception($"No game data hashes URL for hotel: {hotel.Name}.");
            }
        }
        else
        {
            _logger.LogInformation("Fetching hashes from '{Url}'.", hashesUrl);
            var res = await _http.GetAsync(hashesUrl, cancellationToken);
            if (!res.IsSuccessStatusCode)
                _logger.LogWarning("Server responded with {Status} {Reason}.", (int)res.StatusCode, res.ReasonPhrase);
            res.EnsureSuccessStatusCode();

            using var stream = await res.Content.ReadAsStreamAsync(cancellationToken);
            var hashesContainer = JsonSerializer.Deserialize(stream, GameDataJsonContext.Default.GameDataHashesContainer)
                ?? throw new Exception($"Failed to deserialize game data hashes for hotel: {hotel.Name}.");

            fetchedHashes = [];
            foreach (var entry in hashesContainer.Hashes)
            {
                GameDataType? type = GetGameDataTypeFromName(entry.Name);
                if (!type.HasValue)
                    throw new Exception($"Unknown game data type: '{entry.Name}'.");
                fetchedHashes[type.Value] = entry.Hash;
            }
        }

        if (!fetchedHashes.Equals(cachedHashes))
        {
            await UpdateHashesAsync(hotel, fetchedHashes);
        }

        return fetchedHashes;
    }

    public async Task<GameDataDownloadResult> DownloadAsync(
        Hotel hotel, GameDataType type, string hash, CancellationToken cancellationToken = default)
    {
        string cacheFolderPath = Path.Combine(
            _cachePath,
            hotel.Identifier,
            GetSimpleNameForGameDataType(type));

        Directory.CreateDirectory(cacheFolderPath);

        string filePath = Path.Combine(cacheFolderPath, hash);
        FileInfo file = new(filePath);

        if (!file.Exists)
        {
            if (CacheOnly)
                throw new Exception($"{type} file does not exist in cache-only mode.");

            string url = _urlProvider.GetGameDataUrl(hotel, type, hash)
                ?? throw new Exception($"Failed to get URL for {type}.");

            _logger.LogDebug("Fetching {GamedataType} data from '{Url}'.", type, url);

            var res = await _http.GetAsync(url, cancellationToken);
            if (res.StatusCode == HttpStatusCode.TemporaryRedirect &&
                res.Headers.Location is not null)
            {
                if (!TryGetHashFromLocation(res, out string? updatedHash))
                    throw new Exception($"Failed to get updated hash from redirect location: '{res.Headers.Location}'.");

                _logger.LogInformation("Updated {GameDataHash} hash '{OldHash}' -> '{NewHash}'.", type, hash, updatedHash);

                hash = updatedHash;
                res = await _http.GetAsync(res.Headers.Location, cancellationToken);
            }
            res.EnsureSuccessStatusCode();

            using var outs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            await res.Content.CopyToAsync(outs, cancellationToken);
        }
        else
        {
            _logger.LogDebug("Using {GamedataType} from disk cache.", type);
        }

        return new GameDataDownloadResult(
            Type: type,
            Hash: hash,
            FilePath: filePath
        );
    }
}