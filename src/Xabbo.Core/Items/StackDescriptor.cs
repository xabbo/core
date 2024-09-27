namespace Xabbo.Core;

/// <summary>
/// Represents an item type, kind, identifier, and variant,
/// as well as whether the item is tradable and groupable.
/// </summary>
/// <remarks>
/// Similar to <see cref="ItemDescriptor"/>, but intended for inventory item stacks.
/// Enables items to be grouped into inventory stacks.
/// Identifier is included for compatibility with origins.
/// Variant ensures different posters are separated from each other.
/// </remarks>
public readonly record struct StackDescriptor(
    ItemType Type,
    int Kind,
    string? Identifier,
    string? Variant,
    bool IsTradeable,
    bool IsGroupable
);
