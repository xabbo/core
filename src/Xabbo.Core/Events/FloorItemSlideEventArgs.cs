namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.FloorItemSlide"/> event.
/// </summary>
/// <param name="item">The sliding item.</param>
/// <param name="previousLocation">The previous location of the item.</param>
/// <param name="rollerId">The ID of the roller that caused the slide.</param>
public sealed class FloorItemSlideEventArgs(IFloorItem item, Tile previousLocation, Id rollerId)
    : FloorItemEventArgs(item)
{
    /// <summary>
    /// Gets the previous location of the item.
    /// </summary>
    public Tile PreviousLocation { get; } = previousLocation;

    /// <summary>
    /// Gets the ID of the roller that caused the slide.
    /// </summary>
    public Id RollerId = rollerId;
}
