namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the
/// <see cref="Game.RoomManager.FloorItemAdded"/> or
/// <see cref="Game.RoomManager.FloorItemRemoved"/> event.
/// </summary>
/// <param name="item">The item involved in the event.</param>
public class FloorItemEventArgs(IFloorItem item)
{
    /// <summary>
    /// Gets the floor item.
    /// </summary>
    public IFloorItem Item { get; } = item;
}
