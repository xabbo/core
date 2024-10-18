namespace Xabbo.Core;

/// <summary>
/// Represents an item type, kind, identifier, and variant.
/// </summary>
/// <remarks>
/// Enables items to be grouped together by using the descriptor as a key.
/// Identifier is included for compatibility with Origins.
/// Variant ensures different posters are separated from each other.
/// </remarks>
public readonly record struct ItemDescriptor(ItemType Type, int Kind, string? Identifier, string? Variant) : IItem
{
    readonly Id IItem.Id => -1;

    public override string ToString() => Type switch
    {
        ItemType.Floor or ItemType.Wall => !string.IsNullOrWhiteSpace(Variant) ? $"{Type}:{Kind}:{Variant}" : $"{Type}:{Kind}",
        ItemType.Badge or ItemType.Effect or ItemType.Bot => $"{Type}:{Variant ?? "?"}",
        _ => $"{Type}:{Kind}:{Variant ?? "?"}"
    };

    public static implicit operator ItemDescriptor((ItemType Type, int Kind, string Variant) tuple) => new(tuple.Type, tuple.Kind, null, tuple.Variant);
    public static implicit operator ItemDescriptor((ItemType Type, string Identifier, string Variant) tuple) => new(tuple.Type, 0, tuple.Identifier, tuple.Variant);
    public static implicit operator ItemDescriptor((ItemType Type, int Kind) tuple) => new(tuple.Type, tuple.Kind, null, null);
}
