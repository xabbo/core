namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.RoomDataUpdated"/> event.
/// </summary>
/// <param name="data">The room data.</param>
public sealed class RoomDataEventArgs(IRoomData data)
{
    /// <summary>
    /// Gets the room data.
    /// </summary>
    public IRoomData Data { get; } = data;
}
