using System.Threading;
using System.Threading.Tasks;

namespace Xabbo.Core.GameData;

public interface IGameDataLoader
{
    /// <summary>
    /// Loads the game data hashes for the specified hotel.
    /// </summary>
    Task<GameDataHashes> LoadHashesAsync(Hotel hotel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the updated game data hashes to the cache.
    /// </summary>
    Task UpdateHashesAsync(Hotel hotel, GameDataHashes hashes);

    /// <summary>
    /// Downloads game data for the specified hotel, game data type and hash, then returns the updated
    /// hash and the file path to the downloaded data.
    /// </summary>
    Task<GameDataDownloadResult> DownloadAsync(Hotel hotel, GameDataType type, string hash, CancellationToken cancellationToken = default);
}