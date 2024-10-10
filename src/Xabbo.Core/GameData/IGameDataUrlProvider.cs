namespace Xabbo.Core.GameData;

/// <summary>
/// Provides URL resource locations for game data.
/// </summary>
public interface IGameDataUrlProvider
{
    /// <summary>
    /// Gets the game data hashes URL for the specified hotel.
    /// If a hashes endpoint does not exist for the hotel, <c>null</c> will be returned.
    /// </summary>
    string? GetHashesUrl(Hotel hotel);

    /// <summary>
    /// Gets the URL for the specified hotel, game data type and hash.
    /// </summary>
    string? GetGameDataUrl(Hotel hotel, GameDataType type, string? hash = null);
}