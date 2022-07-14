using System;

namespace Xabbo.Core;

/// <summary>
/// Represents an item type, kind, and variant (used for posters).
/// </summary>
public struct ItemDescriptor : IItem
{
    long IItem.Id => -1;

    /// <summary>
    /// Gets the item type.
    /// </summary>
    public ItemType Type { get; }
    /// <summary>
    /// Gets the item kind.
    /// </summary>
    public int Kind { get; }
    /// <summary>
    /// Gets the item variant.
    /// </summary>
    public string Variant { get; }

    public ItemDescriptor(ItemType type, int kind)
        : this(type, kind, null)
    { }

    public ItemDescriptor(ItemType type, int kind, string? variant)
    {
        Type = type;
        Kind = kind;
        Variant = variant ?? string.Empty;
    }

    public override int GetHashCode() => (Type, Kind, Variant).GetHashCode();

    public override bool Equals(object? obj)
    {
        return obj is ItemDescriptor other
            && Equals(other);
    }

    public bool Equals(ItemDescriptor other)
    {
        return
            Type == other.Type &&
            Kind == other.Kind &&
            Variant == other.Variant;
    }

    public static bool operator ==(ItemDescriptor a, ItemDescriptor b) => a.Equals(b);
    public static bool operator !=(ItemDescriptor a, ItemDescriptor b) => !(a == b);
}
