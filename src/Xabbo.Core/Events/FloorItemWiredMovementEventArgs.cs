namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Xabbo.Core.Game.RoomManager.FloorItemWiredMovement"/> event.
/// </summary>
/// <param name="item">The floor item that was moved.</param>
/// <param name="previousLocation">The previous location of the floor item.</param>
/// <param name="previousDirection">The previous direction of the floor item.</param>
/// <param name="movement">The wired movement that caused the event.</param>
public sealed class FloorItemWiredMovementEventArgs(
    IFloorItem item,
    Tile previousLocation,
    int previousDirection,
    FloorItemWiredMovement movement
)
    : FloorItemEventArgs(item)
{
    /// <summary>
    /// Gets the previous location of the floor item.
    /// </summary>
    public Tile PreviousLocation { get; } = previousLocation;

    /// <summary>
    /// Gets the previous direction of the floor item.
    /// </summary>
    public int PreviousDirection { get; } = previousDirection;

    /// <summary>
    /// Gets the wired movement that caused this event.
    /// </summary>
    public FloorItemWiredMovement Movement { get; } = movement;
}