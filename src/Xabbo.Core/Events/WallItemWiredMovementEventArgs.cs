namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Xabbo.Core.Game.RoomManager.WallItemWiredMovement"/> event.
/// </summary>
/// <param name="item">The wall item that was moved.</param>
/// <param name="previousLocation">The previous location of the wall item.</param>
/// <param name="movement">The wired movement that caused the event.</param>
public sealed class WallItemWiredMovementEventArgs(
    IWallItem item,
    WallLocation previousLocation,
    WallItemWiredMovement movement
)
    : WallItemEventArgs(item)
{
    /// <summary>
    /// Gets the previous location of the floor item.
    /// </summary>
    public WallLocation PreviousLocation { get; } = previousLocation;

    /// <summary>
    /// Gets the wired movement that caused this event.
    /// </summary>
    public WallItemWiredMovement Movement { get; } = movement;
}