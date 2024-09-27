using System.Linq;
using System.Collections.Generic;

namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the
/// <see cref="Game.RoomManager.AvatarsAdded"/> or
/// <see cref="Game.RoomManager.AvatarsUpdated"/> event.
/// </summary>
/// <param name="avatars">The avatars.</param>
public sealed class AvatarsEventArgs(IEnumerable<IAvatar> avatars)
{
    /// <summary>
    /// Gets the avatars.
    /// </summary>
    public IAvatar[] Avatars { get; } = avatars.ToArray();
}
