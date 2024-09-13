using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Xabbo.Core.Serialization;
using Xabbo.Core.Web;

namespace Xabbo.Core.GameData;

public class GameDataManager : IGameDataManager
{
    private static string GetDefaultCachePath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return @"%APPDATA%\xabbo\cache\gamedata";
        }
        else
        {
            return "%HOME%/.xabbo/cache/gamedata";
        }
    }

    private readonly SemaphoreSlim _loadSemaphore = new(1, 1);

    private TaskCompletionSource? _tcsLoad;
    private Task _loadTask;

    public string CachePath { get; }

    public FigureData? Figure { get; private set; }
    public FurniData? Furni { get; private set; }
    public ProductData? Products { get; private set; }
    public ExternalTexts? Texts { get; private set; }

    public event Action? Loading;
    public event Action? Loaded;
    public event Action<Exception>? LoadFailed;

    public GameDataManager()
        : this(GetDefaultCachePath())
    { }

    public GameDataManager(string cachePath)
    {
        _tcsLoad = new(TaskCreationOptions.RunContinuationsAsynchronously);
        _loadTask = _tcsLoad.Task;

        CachePath = Path.GetFullPath(Environment.ExpandEnvironmentVariables(cachePath));
    }

    private void Reset()
    {
        Figure = null;
        Furni = null;
        Products = null;
        Texts = null;
    }

    private async Task LoadDataAsync(
        HttpClient http,
        Hotel hotel,
        GameDataType type,
        string url,
        string? hash,
        CancellationToken cancellationToken)
    {
        string cacheFolderPath = Path.Combine(CachePath, hotel.Identifier, type.ToString().ToLower());
        Directory.CreateDirectory(cacheFolderPath);

        string filePath = Path.Combine(cacheFolderPath, hash ?? "1");

        FileInfo file = new FileInfo(filePath);

        if (!file.Exists || (hash is null && (DateTime.Now - file.LastWriteTime).TotalHours > 3))
        {
            url = $"{url}/{hash ?? "1"}";
            HttpResponseMessage response = await http.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var outs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            await response.Content.CopyToAsync(outs, cancellationToken);
        }

        switch (type)
        {
            case GameDataType.Furni: Furni = FurniData.LoadJsonFile(filePath); break;
            case GameDataType.Product: Products = ProductData.LoadJsonFile(filePath); break;
            case GameDataType.Texts: Texts = ExternalTexts.Load(filePath); break;
            case GameDataType.Figure: Figure = FigureData.LoadXml(filePath); break;
            default: break;
        }
    }

    public async Task LoadAsync(Hotel hotel, CancellationToken cancellationToken = default)
    {
        if (!_loadSemaphore.Wait(0, cancellationToken))
            throw new InvalidOperationException("Game data is currently being loaded.");

        try
        {
            if (_tcsLoad is null)
            {
                _tcsLoad = new(TaskCreationOptions.RunContinuationsAsynchronously);
                _loadTask = _tcsLoad.Task;
            }

            Reset();
            Loading?.Invoke();

            string hotelCachePath = Path.Combine(CachePath, hotel.Identifier);
            Directory.CreateDirectory(hotelCachePath);

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("User-Agent", H.UserAgent);

            List<GameDataHash>? hashes = null;

            if (hotel.IsOrigins)
            {
                string baseUrl = $"https://{hotel.Subdomain}-gamedata.habbo.{hotel.Domain}";
                hashes = [
                    new GameDataHash
                    {
                        Name = "external_texts",
                        Url = $"{baseUrl}/external_texts"
                    },
                    // new GameDataHash
                    // {
                    //     Name = "figurepartlist",
                    //     Url = $"{baseUrl}/figuredata"
                    // }
                ];
            }
            else
            {
                GameDataHashesContainer? container = null;
                FileInfo hashesFile = new(Path.Combine(hotelCachePath, "hashes.json"));
                if (hashesFile.Exists &&
                    (DateTime.Now - hashesFile.LastWriteTime) < TimeSpan.FromMinutes(15))
                {
                    container = JsonSerializer.Deserialize(
                        await File.ReadAllTextAsync(hashesFile.FullName, cancellationToken),
                        JsonContext.Default.GameDataHashesContainer
                    );
                }
                else
                {
                    string json = await http.GetStringAsync(
                        $"https://{hotel.WebHost}/gamedata/hashes2",
                        cancellationToken
                    );
                    container = JsonSerializer.Deserialize(json, JsonContext.Default.GameDataHashesContainer);

                    await File.WriteAllTextAsync(hashesFile.FullName, json, cancellationToken);
                }

                if (container is not null)
                    hashes = container.Hashes;
            }

            if (hashes is null)
                throw new Exception("Failed to deserialize game data hashes.");

            List<Task> loadTasks = [];

            foreach (GameDataHash entry in hashes)
            {
                GameDataType type = entry.Name switch
                {
                    "furnidata" => GameDataType.Furni,
                    "productdata" => GameDataType.Product,
                    "external_texts" => GameDataType.Texts,
                    "figurepartlist" => GameDataType.Figure,
                    _ => (GameDataType)(-1)
                };

                if (!Enum.IsDefined(type)) continue;

                loadTasks.Add(LoadDataAsync(http, hotel, type, entry.Url, entry.Hash, cancellationToken));
            }

            await Task.WhenAll(loadTasks);

            if (hotel.IsOrigins && Texts is not null)
                Furni = FurniData.FromOriginsTexts(Texts);

            if (Furni is not null && Texts is not null)
                Extensions.Initialize(Furni, Texts);

            Loaded?.Invoke();
            _tcsLoad.TrySetResult();
        }
        catch (Exception ex)
        {
            Reset();
            LoadFailed?.Invoke(ex);
            _tcsLoad?.TrySetException(ex);
            throw;
        }
        finally
        {
            _tcsLoad = null;
            _loadSemaphore.Release();
        }
    }

    public async Task WaitForLoadAsync(CancellationToken cancellationToken = default)
    {
        await await Task.WhenAny(_loadTask, Task.Delay(-1, cancellationToken));
    }
}
