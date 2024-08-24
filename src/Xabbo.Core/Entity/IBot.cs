using System.Collections.Generic;

namespace Xabbo.Core;

public interface IBot : IEntity
{
    /// <summary>
    /// Gets the gender of the bot.
    /// </summary>
    Gender Gender { get; }

    /// <summary>
    /// Gets the owner of the bot's ID.
    /// </summary>
    Id OwnerId { get; }

    /// <summary>
    /// Gets the owner of the bot's name.
    /// </summary>
    string OwnerName { get; }

    /// <summary>
    /// Gets the data of the bot.
    /// </summary>
    IReadOnlyList<short> Data { get; }
}
