using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Represents a bot in a room.
/// </summary>
public interface IBot : IAvatar
{
    /// <summary>
    /// The gender of the bot.
    /// </summary>
    Gender Gender { get; }

    /// <summary>
    /// The ID of the owner of the bot.
    /// </summary>
    Id OwnerId { get; }

    /// <summary>
    /// The name of the owner of the bot.
    /// </summary>
    string OwnerName { get; }

    IReadOnlyList<short> Data { get; }
}
