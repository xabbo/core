namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.FloorItemDataUpdated"/> event.
/// </summary>
/// <param name="item">The floor item.</param>
/// <param name="previousData">The previous state of the updated floor item's data.</param>
/// <remarks>
/// <see cref="FloorItemEventArgs.Item"/> contains the updated state of the item.
/// </remarks>
public sealed class FloorItemDataUpdatedEventArgs(IFloorItem item, IItemData previousData)
    : FloorItemEventArgs(item)
{
    /// <summary>
    /// Gets the previous state of the updated floor item's data.
    /// </summary>
    public IItemData PreviousData { get; } = previousData;
}
