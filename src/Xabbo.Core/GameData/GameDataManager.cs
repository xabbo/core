using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Xabbo.Core.GameData;

/// <summary>
/// Manages the game data for a specified hotel.
/// </summary>
public class GameDataManager : IGameDataManager
{
    private readonly ILogger _logger;
    private readonly IGameDataLoader _loader;

    private readonly SemaphoreSlim _loadSemaphore = new(1, 1);
    private TaskCompletionSource? _tcsLoad;
    private Task _loadTask;

    public FigureData? Figure { get; private set; }
    public FurniData? Furni { get; private set; }
    public ProductData? Products { get; private set; }
    public ExternalTexts? Texts { get; private set; }

    public event Action? Loading;
    public event Action? Loaded;
    public event Action<Exception>? LoadFailed;

    public bool AutoInitCoreExtensions { get; set; } = true;

    public GameDataManager(
        IGameDataLoader? loader = null,
        ILoggerFactory? loggerFactory = null)
    {
        _loader = loader ?? new GameDataLoader(loggerFactory: loggerFactory);
        _logger = (ILogger?)loggerFactory?.CreateLogger<GameDataManager>() ?? NullLogger.Instance;

        _tcsLoad = new(TaskCreationOptions.RunContinuationsAsynchronously);
        _loadTask = _tcsLoad.Task;
    }

    private void Reset()
    {
        Figure = null;
        Furni = null;
        Products = null;
        Texts = null;
    }

    /// <summary>
    /// Loads the specified game data types.
    /// If no types are specified, then all types are loaded.
    /// </summary>
    /// <param name="hotel">The hotel to load game data for.</param>
    /// <param name="typesToLoad">The game data types to load.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="InvalidOperationException">If game data is currently being loaded.</exception>
    public async Task LoadAsync(Hotel hotel, GameDataType[]? typesToLoad = null, CancellationToken cancellationToken = default)
    {
        if (!_loadSemaphore.Wait(0, cancellationToken))
            throw new InvalidOperationException("Game data is currently being loaded.");

        try
        {
            _logger.LogInformation("Loading game data for {Hotel}.", hotel.Name);

            if (_tcsLoad is null)
            {
                _tcsLoad = new(TaskCreationOptions.RunContinuationsAsynchronously);
                _loadTask = _tcsLoad.Task;
            }

            Reset();
            Loading?.Invoke();

            GameDataHashes hashes = await _loader.LoadHashesAsync(hotel, cancellationToken);

            List<Task<GameDataDownloadResult>> loadTasks = [];

            foreach (var (type, hash) in hashes)
            {
                if (typesToLoad?.Contains(type) == false)
                    continue;

                loadTasks.Add(_loader.DownloadAsync(hotel, type, hash, cancellationToken));
            }

            await Task.WhenAll(loadTasks);
            _logger.LogDebug("All load tasks completed.");

            bool updateHashes = false;

            foreach (var task in loadTasks)
            {
                var (type, updatedHash, filePath) = await task;

                if (hashes[type] != updatedHash)
                {
                    hashes[type] = updatedHash;
                    updateHashes = true;
                }

                switch (type)
                {
                    case GameDataType.FigureData:
                        if (hotel.IsOrigins)
                            Figure = FigureData.LoadJsonOrigins(filePath);
                        else
                            Figure = FigureData.LoadXml(filePath);
                        break;
                    case GameDataType.FurniData:
                        if (hotel.IsOrigins)
                            Furni = FurniData.LoadJsonOriginsFile(filePath);
                        else
                            Furni = FurniData.LoadJsonFile(filePath);
                        break;
                    case GameDataType.ExternalTexts:
                        Texts = ExternalTexts.Load(filePath);
                        break;
                    case GameDataType.ProductData:
                        Products = ProductData.LoadJsonFile(filePath);
                        break;
                }
            }

            // if (hotel.IsOrigins && Texts is not null && typesToLoad?.Contains(GameDataType.FurniData) != false)
            //     Furni = FurniData.FromOriginsTexts(Texts);

            if (updateHashes)
                await _loader.UpdateHashesAsync(hotel, hashes);

            if (AutoInitCoreExtensions)
                Extensions.Initialize(Furni, Texts);

            _logger.LogInformation("Game data loaded.");
            Loaded?.Invoke();
            _tcsLoad.TrySetResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load game data: {Message}", ex.Message);

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

    /// <summary>
    /// Waits for the game data to load.
    /// </summary>
    public async Task WaitForLoadAsync(CancellationToken cancellationToken = default)
    {
        await await Task.WhenAny(_loadTask, Task.Delay(-1, cancellationToken));
    }
}
