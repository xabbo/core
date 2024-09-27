namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.FloorItemUpdated"/> event.
/// </summary>
/// <param name="previous">The previous state of the item.</param>
/// <param name="updated">The updated state of the item.</param>
/// <remarks>
/// <see cref="FloorItemEventArgs.Item"/> contains the updated state of the item.
/// </remarks>
public sealed class FloorItemUpdatedEventArgs(IFloorItem previous, IFloorItem updated)
    : FloorItemEventArgs(updated)
{
    /// <summary>
    /// Gets the previous state of the item.
    /// </summary>
    public IFloorItem PreviousItem { get; } = previous;
}
