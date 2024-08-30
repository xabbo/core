using Xabbo.Messages;

using Xabbo.Core.Game;

namespace Xabbo.Core;

/// <summary>
/// Represents a room furniture.
/// </summary>
public interface IFurni : IItem, IComposer
{
    /// <summary>
    /// Gets the owner of the furni's ID.
    /// </summary>
    Id OwnerId { get; }
    /// <summary>
    /// Gets the owner of the furni's name.
    /// </summary>
    string OwnerName { get; }

    /// <summary>
    /// Gets the state of the furni from its data if the data string is an integer, otherwise returns <c>-1</c>.
    /// </summary>
    int State { get; }

    /// <summary>
    /// Gets the number of seconds until the furni expires.
    /// </summary>
    int SecondsToExpiration { get; }
    /// <summary>
    /// Gets the usage policy of the furni.
    /// </summary>
    FurniUsage Usage { get; }

    /// <summary>
    /// Gets the class name of the furni.
    /// </summary>
    string Identifier { get; }

    /// <summary>
    /// Gets whether the furni is hidden client-side or not by the <see cref="RoomManager"/>.
    /// </summary>
    bool IsHidden { get; }
}
