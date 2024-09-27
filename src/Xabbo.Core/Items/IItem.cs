namespace Xabbo.Core;

/// <summary>
/// Represents an item.
/// </summary>
public interface IItem
{
    /// <summary>
    /// The type of the item.
    /// </summary>
    ItemType Type { get; }

    /// <summary>
    /// The kind of the item.
    /// </summary>
    int Kind { get; }

    /// <summary>
    /// The identifier of the item.
    /// </summary>
    string? Identifier { get; }

    /// <summary>
    /// The item's ID.
    /// </summary>
    Id Id { get; }
}
