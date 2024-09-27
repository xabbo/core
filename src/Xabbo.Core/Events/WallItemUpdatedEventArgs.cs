namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.WallItemUpdated"/> event.
/// </summary>
/// <param name="previousItem">The previous wall item.</param>
/// <param name="updatedItem">The updated wall item.</param>
/// <remarks>
/// <see cref="WallItemEventArgs.Item"/> contains the updated state of the item.
/// </remarks>
public sealed class WallItemUpdatedEventArgs(IWallItem previousItem, IWallItem updatedItem)
    : WallItemEventArgs(updatedItem)
{
    /// <summary>
    /// The previous state of the wall item before the update.
    /// </summary>
    public IWallItem PreviousItem { get; } = previousItem;
}
