namespace Xabbo.Core;

public readonly record struct StackDescriptor(
    ItemType Type,
    int Kind,
    string? Identifier,
    string? Variant,
    bool IsTradeable,
    bool IsGroupable
);
