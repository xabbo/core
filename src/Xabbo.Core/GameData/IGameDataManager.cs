using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xabbo.Core.GameData;

/// <summary>
/// Represents a service that manages game data for a hotel.
/// </summary>
public interface IGameDataManager : IGameDataProvider
{
    /// <summary>
    /// Occurs when game data begins loading.
    /// </summary>
    event Action Loading;

    /// <summary>
    /// Occurs when game data is successfully loaded.
    /// </summary>
    event Action Loaded;

    /// <summary>
    /// Occurs when game data fails to load.
    /// </summary>
    event Action<Exception> LoadFailed;

    /// <summary>
    /// Loads game data for the specified hotel.
    /// </summary>
    Task LoadAsync(Hotel hotel, GameDataType[]? typesToLoader = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Waits for game data to load.
    /// </summary>
    Task WaitForLoadAsync(CancellationToken cancellationToken = default);
}
