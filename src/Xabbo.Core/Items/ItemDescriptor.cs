namespace Xabbo.Core;

/// <summary>
/// Represents an item type, kind, and variant (used for posters).
/// </summary>
public readonly record struct ItemDescriptor(ItemType Type, int Kind = -1, string? Identifier = null, string? Variant = null) : IItem
{
    readonly Id IItem.Id => -1;

    public override string ToString() => Type switch
    {
        ItemType.Floor or ItemType.Wall => !string.IsNullOrWhiteSpace(Variant) ? $"{Type}:{Kind}:{Variant}" : $"{Type}:{Kind}",
        ItemType.Badge or ItemType.Effect or ItemType.Bot => $"{Type}:{Variant ?? "?"}",
        _ => $"{Type}:{Kind}:{Variant ?? "?"}"
    };

    public static implicit operator ItemDescriptor((ItemType Type, int Kind, string Variant) tuple) => new(tuple.Type, tuple.Kind, tuple.Variant);
    public static implicit operator ItemDescriptor((ItemType Type, int Kind) tuple) => new(tuple.Type, tuple.Kind);
}
