namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the
/// <see cref="Game.RoomManager.WallItemAdded"/> or
/// <see cref="Game.RoomManager.WallItemRemoved"/> event.
/// </summary>
/// <param name="item">The wall item.</param>
public class WallItemEventArgs(IWallItem item)
{
    /// <summary>
    /// Gets the wall item.
    /// </summary>
    public IWallItem Item { get; } = item;
}
