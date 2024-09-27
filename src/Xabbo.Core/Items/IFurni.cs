using Xabbo.Core.Game;

namespace Xabbo.Core;

/// <summary>
/// Represents a room furniture.
/// </summary>
public interface IFurni : IItem
{
    /// <summary>
    /// The owner of the furni's ID.
    /// </summary>
    Id OwnerId { get; }

    /// <summary>
    /// The owner of the furni's name.
    /// </summary>
    string OwnerName { get; }

    /// <summary>
    /// Gets the state of the furni from its data if the data string is an integer,
    /// otherwise returns <c>-1</c>.
    /// </summary>
    int State { get; }

    /// <summary>
    /// The number of seconds until the furni expires.
    /// </summary>
    int SecondsToExpiration { get; }

    /// <summary>
    /// The usage policy of the furni.
    /// </summary>
    FurniUsage Usage { get; }

    /// <summary>
    /// Gets whether the furni is hidden client-side by the <see cref="RoomManager"/>.
    /// </summary>
    bool IsHidden { get; }
}
