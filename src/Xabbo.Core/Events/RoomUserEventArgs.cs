namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for events involving a single <see cref="IUser"/>.
/// </summary>
/// <param name="user">The user instance.</param>
public class RoomUserEventArgs(IUser user) : AvatarEventArgs(user)
{
    /// <summary>
    /// Gets the user involved in the event.
    /// </summary>
    public IUser User { get; } = user;
}
