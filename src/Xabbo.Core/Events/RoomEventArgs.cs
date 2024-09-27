using Xabbo.Core.Game;

namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.Entered"/> event.
/// </summary>
/// <param name="room">The room instance.</param>
public sealed class RoomEventArgs(IRoom room)
{
    /// <summary>
    /// Gets the room instance.
    /// </summary>
    public IRoom Room { get; } = room;
}
