using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Xabbo.Core.GameData;

namespace Xabbo.Core;

/// <summary>
/// Provides convenient extension methods utilizing Xabbo.Core.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class Extensions
{
    private static FurniData? _furniData;
    private static ExternalTexts? _texts;

    private static FurniData FurniData => _furniData ?? throw new InvalidOperationException($"{typeof(Extensions).FullName} has not been initialized.");
    private static ExternalTexts Texts => _texts ?? throw new InvalidOperationException($"{typeof(Extensions).FullName} has not been initialized.");

    /// <summary>
    /// Gets whether xabbo core extensions have been initialized.
    /// </summary>
    public static bool IsInitialized { get; private set; }

    /// <summary>
    /// Initializes xabbo core extensions with the specified game data provider.
    /// </summary>
    public static void Initialize(IGameDataProvider provider)
    {
        _furniData = provider.Furni;
        _texts = provider.Texts;

        IsInitialized = _furniData is not null && _texts is not null;
    }

    #region - Texts -
    /// <summary>
    /// Attempts to get a poster name by its variant from the external texts.
    /// </summary>
    public static bool TryGetPosterName(this ExternalTexts texts, string variant, [NotNullWhen(true)] out string? name)
        => texts.TryGetValue($"poster_{variant}_name", out name);

    /// <summary>
    /// Attempts to get a poster description by its variant from the external texts.
    /// </summary>
    public static bool TryGetPosterDescription(this ExternalTexts texts, string variant, [NotNullWhen(true)] out string? description)
        => texts.TryGetValue($"poster_{variant}_desc", out description);

    /// <summary>
    /// Attempts to get a badge name by its code from the external texts.
    /// </summary>
    public static bool TryGetBadgeName(this ExternalTexts texts, string code, [NotNullWhen(true)] out string? name)
        => texts.TryGetValue($"badge_name_{code}", out name) || texts.TryGetValue($"{code}_badge_name", out name);

    /// <summary>
    /// Gets a badge name by its code from the external texts. Returns <c>null</c> if it is not found.
    /// </summary>
    public static string? GetBadgeName(this ExternalTexts texts, string code)
        => TryGetBadgeName(texts, code, out string? name) ? name : null;

    /// <summary>
    /// Attempts to get a badge description by its code from the external texts.
    /// </summary>
    public static bool TryGetBadgeDescription(this ExternalTexts texts, string code, [NotNullWhen(true)] out string? description)
        => texts.TryGetValue($"badge_desc_{code}", out description);

    /// <summary>
    /// Gets a badge description by its code from the external texts. Returns <c>null</c> if it is not found.
    /// </summary>
    public static string? GetBadgeDescription(this ExternalTexts texts, string code)
        => TryGetBadgeDescription(texts, code, out string? description) ? description : null;

    /// <summary>
    /// Attempts to get an effect name by its ID from the external texts.
    /// </summary>
    public static bool TryGetEffectName(this ExternalTexts texts, int id, [NotNullWhen(true)] out string? name)
        => texts.TryGetValue($"fx_{id}", out name);

    /// <summary>
    /// Gets an effect name by its ID from the external texts. Returns <c>null</c> if it is not found.
    /// </summary>
    public static string? GetEffectName(this ExternalTexts texts, int id)
        => TryGetEffectName(texts, id, out string? name) ? name : null;

    /// <summary>
    /// Attempts to get an effect description by its ID from the external texts.
    /// </summary>
    public static bool TryGetEffectDescription(this ExternalTexts texts, int id, [NotNullWhen(true)] out string? description)
        => texts.TryGetValue($"fx_{id}_desc", out description);

    /// <summary>
    /// Gets an effect description by its ID from the external texts. Returns <c>null</c> if it is not found.
    /// </summary>
    public static string? GetEffectDescription(this ExternalTexts texts, int id)
        => TryGetEffectDescription(texts, id, out string? description) ? description : null;

    /// <summary>
    /// Attempts to get a hand item name by its ID from the external texts.
    /// </summary>
    public static bool TryGetHandItemName(this ExternalTexts texts, int id, [NotNullWhen(true)] out string? name)
        => texts.TryGetValue($"handitem{id}", out name);

    /// <summary>
    /// Gets a hand item name by its ID from the external texts. Returns <c>null</c> if it is not found.
    /// </summary>
    public static string? GetHandItemName(this ExternalTexts texts, int id)
        => TryGetHandItemName(texts, id, out string? name) ? name : null;

    /// <summary>
    /// Gets all hand item IDs matching the specified name from the external texts.
    /// </summary>
    public static IEnumerable<int> GetHandItemIds(this ExternalTexts texts, string name)
    {
        foreach (var (key, value) in texts
            .Where(x => x.Value.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            if (key.StartsWith("handitem"))
                if (int.TryParse(key[8..], out int id))
                    yield return id;
        }
    }
    #endregion

    #region - Items -
    /// <summary>
    /// Gets the name of an item or a generic type/kind specifier if unavailable.
    /// </summary>
    internal static string GetNameOrDefault(this IItem item)
    {
        if (TryGetName(item, out string? name))
            return name;

        TryGetVariant(item, out string? variant);

        return item.Type switch
        {
            ItemType.Effect or ItemType.Badge or ItemType.Bot => $"{item.Type}:{variant ?? "?"}",
            _ => string.IsNullOrWhiteSpace(variant) ? $"{item.Type}:{item.Kind}" : $"{item.Type}:{item.Kind}:{variant}"
        };
    }

    /// <inheritdoc cref="FurniData.HasVariant(IItem)" />
    public static bool HasVariant(this IItem item) => FurniData.HasVariant(item);

    /// <inheritdoc cref="FurniData.GetVariant(IItem)" />
    public static string? GetVariant(this IItem item) => FurniData.GetVariant(item);

    /// <inheritdoc cref="FurniData.TryGetVariant(IItem, out string?)" />
    public static bool TryGetVariant(this IItem item, [NotNullWhen(true)] out string? variant) => FurniData.TryGetVariant(item, out variant);

    /// <inheritdoc cref="FurniData.GetInfo(IItem)" />
    public static FurniInfo GetInfo(this IItem item) => FurniData.GetInfo(item);

    /// <inheritdoc cref="FurniData.TryGetInfo(IItem, out FurniInfo?)" />
    public static bool TryGetInfo(this IItem item, [NotNullWhen(true)] out FurniInfo? info) => FurniData.TryGetInfo(item, out info);

    /// <summary>
    /// Gets the identifier of this item.
    /// </summary>
    public static string GetIdentifier(this IItem item) => item.Identifier ?? GetInfo(item).Identifier;

    /// <summary>
    /// Gets the identifier of this item.
    /// </summary>
    public static bool TryGetIdentifier(this IItem item, [NotNullWhen(true)] out string? identifier)
    {
        if (item.Identifier is not null)
        {
            identifier = item.Identifier;
            return true;
        }
        else if (TryGetInfo(item, out FurniInfo? info))
        {
            identifier = info.Identifier;
            return true;
        }
        else
        {
            identifier = null;
            return false;
        }
    }

    /// <summary>
    /// Gets the descriptor of this item.
    /// </summary>
    public static ItemDescriptor GetDescriptor(this IItem item)
    {
        if (item is ItemDescriptor descriptor)
        {
            return descriptor;
        }

        if (!TryGetVariant(item, out string? variant))
            variant = null;

        return new ItemDescriptor(item.Type, item.Kind, item.Identifier, variant);
    }

    public static bool TryGetName(this IItem item, [NotNullWhen(true)] out string? name)
    {
        if (!IsInitialized)
        {
            name = null;
            return false;
        }

        string? variant;

        switch (item.Type)
        {
            case ItemType.Floor or ItemType.Wall:
                FurniData.TryGetInfo(item, out FurniInfo? info);
                if ((item.Identifier == "poster" || info?.Identifier == "poster") &&
                    item.TryGetVariant(out variant))
                {
                    if (Texts.TryGetPosterName(variant, out string? posterName))
                    {
                        name = posterName;
                    }
                    else
                    {
                        name = $"poster_{variant}";
                    }
                    return true;
                }
                else if (info is not null)
                {
                    name = string.IsNullOrWhiteSpace(info.Name) ? info.Identifier : info.Name;
                    return true;
                }
                break;
            case ItemType.Badge:
                if (TryGetVariant(item, out variant))
                {
                    if (Texts.TryGetBadgeName(variant, out string? badgeName) &&
                        !string.IsNullOrWhiteSpace(badgeName))
                    {
                        name = badgeName;
                    }
                    else
                    {
                        name = variant;
                    }
                    return true;
                }
                break;
            case ItemType.Effect:
                if (TryGetVariant(item, out variant))
                {
                    if (int.TryParse(variant, out int effectId) &&
                        Texts.TryGetEffectName(effectId, out string? effectName))
                    {
                        name = string.IsNullOrWhiteSpace(effectName) ? variant : effectName;
                        return true;
                    }
                }
                break;
        }

        name = null;
        return false;
    }

    public static bool TryGetDescription(this IItem item, [NotNullWhen(true)] out string? description)
    {
        if (!IsInitialized)
        {
            description = null;
            return false;
        }

        string? variant;

        switch (item.Type)
        {
            case ItemType.Floor or ItemType.Wall:
                FurniData.TryGetInfo(item, out FurniInfo? info);
                if ((item.Identifier == "poster" ||
                    info?.Identifier == "poster") &&
                    item.TryGetVariant(out variant))
                {
                    if (Texts.TryGetPosterDescription(variant, out description))
                    {
                        return !string.IsNullOrWhiteSpace(description);
                    }
                }
                else if (info is not null)
                {
                    description = info.Description;
                    return !string.IsNullOrWhiteSpace(description);
                }
                break;
            case ItemType.Badge:
                if (TryGetVariant(item, out variant) &&
                    Texts.TryGetBadgeDescription(variant, out description))
                {
                    return !string.IsNullOrWhiteSpace(description);
                }
                break;
            case ItemType.Effect:
                if (TryGetVariant(item, out variant) &&
                    int.TryParse(variant, out int effectId) &&
                    Texts.TryGetEffectDescription(effectId, out description))
                {
                    return !string.IsNullOrWhiteSpace(description);
                }
                break;
        }

        description = null;
        return false;
    }

    /// <summary>
    /// Gets the name of this item.
    /// </summary>
    public static string GetName(this IItem item) => GetNameOrDefault(item);

    /// <summary>
    /// Gets the category of this item.
    /// </summary>
    public static FurniCategory GetCategory(this IItem item) => GetInfo(item).Category;

    /// <summary>
    /// Gets the category name of this item.
    /// </summary>
    public static string GetCategoryName(this IItem item) => GetInfo(item).CategoryName;

    /// <summary>
    /// Gets the furni line of this item.
    /// </summary>
    public static string GetLine(this IItem item) => GetInfo(item).Line;

    /// <summary>
    /// Selects floor items.
    /// </summary>
    public static IEnumerable<T> GetFloorItems<T>(this IEnumerable<T> items)
        where T : IItem => items.Where(x => x.Type == ItemType.Floor);

    /// <summary>
    /// Selects wall items.
    /// </summary>
    public static IEnumerable<T> GetWallItems<T>(this IEnumerable<T> items)
        where T : IItem => items.Where(x => x.Type == ItemType.Wall);

    /// <summary>
    /// Selects items of the specified type, kind and variant.
    /// </summary>
    public static IEnumerable<T> OfKind<T>(this IEnumerable<T> items, ItemDescriptor descriptor)
        where T : IItem
    {
        return items.Where(item =>
            item.Type == descriptor.Type &&
            item.Kind == descriptor.Kind &&
            (descriptor.Variant is null || (
                TryGetVariant(item, out string? variant) && variant.Equals(descriptor.Variant)
            ))
        );
    }

    /// <summary>
    /// Selects items of the same kind as the specified furni info.
    /// </summary>
    public static IEnumerable<T> OfKind<T>(this IEnumerable<T> items, FurniInfo furniInfo)
        where T : IItem => OfKind(items, (furniInfo.Type, furniInfo.Kind));

    /// <summary>
    /// Selects items of the same kind as any of the specified furni info.
    /// </summary>
    public static IEnumerable<T> OfKind<T>(this IEnumerable<T> items, IEnumerable<FurniInfo> furniInfo) where T : IItem
    {
        var set = new HashSet<(ItemType, int)>(furniInfo.Select(info => (info.Type, info.Kind)));
        return items.Where(item => set.Contains((item.Type, item.Kind)));
    }

    /// <summary>
    /// Selects items of the same kind as any of the specified furni info.
    /// </summary>
    public static IEnumerable<T> OfKind<T>(this IEnumerable<T> items, params FurniInfo[] furniInfo)
        where T : IItem => OfKind(items, (IEnumerable<FurniInfo>)furniInfo);

    /// <summary>
    /// Selects items with the specified furni identifier.
    /// </summary>
    public static IEnumerable<T> OfKind<T>(this IEnumerable<T> items, string identifier)
        where T : IItem
    {
        return items.Where(item =>
            FurniData.TryGetInfo(item, out FurniInfo? info) &&
            info.Identifier.Equals(identifier, StringComparison.OrdinalIgnoreCase)
        );
    }

    /// <summary>
    /// Selects items with any of the specified furni identifiers.
    /// </summary>
    public static IEnumerable<T> OfKind<T>(this IEnumerable<T> items, IEnumerable<string> identifiers)
        where T : IItem
    {
        HashSet<string> set = new(identifiers, StringComparer.OrdinalIgnoreCase);
        return items.Where(item =>
            FurniData.TryGetInfo(item, out FurniInfo? info) &&
            set.Contains(info.Identifier)
        );
    }

    /// <summary>
    /// Selects items with any of the specified furni identifiers.
    /// </summary>
    public static IEnumerable<T> OfKind<T>(this IEnumerable<T> items, params string[] identifiers)
        where T : IItem => OfKind(items, (IEnumerable<string>)identifiers);

    /// <summary>
    /// Selects items without the specified identifier.
    /// </summary>
    public static IEnumerable<T> NotOfKind<T>(this IEnumerable<T> items, string identifier)
        where T : IItem
    {
        return items.Where(item =>
            FurniData.TryGetInfo(item, out FurniInfo? info) &&
            !info.Identifier.Equals(identifier, StringComparison.OrdinalIgnoreCase)
        );
    }

    /// <summary>
    /// Selects items without any of the specified identifiers.
    /// </summary>
    public static IEnumerable<T> NotOfKind<T>(this IEnumerable<T> items, IEnumerable<string> identifiers)
        where T : IItem
    {
        HashSet<string> set = new(identifiers, StringComparer.OrdinalIgnoreCase);
        return items.Where(item =>
            FurniData.TryGetInfo(item, out FurniInfo? info) &&
            !set.Contains(info.Identifier)
        );
    }

    /// <inheritdoc cref="NotOfKind{T}(IEnumerable{T}, IEnumerable{string})"/>
    public static IEnumerable<T> NotOfKind<T>(this IEnumerable<T> items, params string[] identifiers)
        where T : IItem => NotOfKind(items, (IEnumerable<string>)identifiers);

    /// <summary>
    /// Selects items of the specified variant.
    /// </summary>
    public static IEnumerable<T> OfVariant<T>(this IEnumerable<T> items, string variant)
        where T : IItem
    {
        return items.Where(item => TryGetVariant(item, out string? v) && variant.Equals(v));
    }

    /// <summary>
    /// Selects items of any of the specified variants.
    /// </summary>
    public static IEnumerable<T> OfVariant<T>(this IEnumerable<T> items, IEnumerable<string> variants)
        where T : IItem
    {
        HashSet<string> set = new(variants, StringComparer.OrdinalIgnoreCase);
        return items.Where(item =>
            TryGetVariant(item, out string? variant) &&
            set.Contains(variant)
        );
    }

    /// <inheritdoc cref="OfVariant{T}(IEnumerable{T}, IEnumerable{string})"/>
    public static IEnumerable<T> OfVariant<T>(this IEnumerable<T> items, params string[] variants)
        where T : IItem => OfVariant(items, (IEnumerable<string>)variants);

    /// <summary>
    /// Selects items belonging to the specified category.
    /// </summary>
    public static IEnumerable<T> OfCategory<T>(this IEnumerable<T> items, FurniCategory category)
        where T : IItem => items.Where(item => item.GetCategory() == category);

    /// <summary>
    /// Selects items belonging to any of the specified categories.
    /// </summary>
    public static IEnumerable<T> OfCategory<T>(this IEnumerable<T> items, IEnumerable<FurniCategory> categories)
        where T : IItem
    {
        HashSet<FurniCategory> set = new(categories);
        return items.Where(item => set.Contains(item.GetCategory()));
    }

    /// <inheritdoc cref="OfCategory{T}(IEnumerable{T}, IEnumerable{FurniCategory})"/>
    public static IEnumerable<T> OfCategory<T>(this IEnumerable<T> items, params FurniCategory[] categories)
        where T : IItem => OfCategory(items, (IEnumerable<FurniCategory>)categories);

    /// <summary>
    /// Selects items with the specified category name.
    /// </summary>
    public static IEnumerable<T> OfCategory<T>(this IEnumerable<T> items, string categoryName)
        where T : IItem
    {
        return items.Where(item => item.GetCategoryName().Equals(categoryName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Selects items with any of the specified category names.
    /// </summary>
    public static IEnumerable<T> OfCategory<T>(this IEnumerable<T> items, IEnumerable<string> categoryNames)
        where T : IItem
    {
        HashSet<string> set = new(categoryNames, StringComparer.OrdinalIgnoreCase);
        return items.Where(item => set.Contains(item.GetCategoryName()));
    }

    /// <inheritdoc cref="OfCategory{T}(IEnumerable{T}, IEnumerable{string})"/>
    public static IEnumerable<T> OfCategory<T>(this IEnumerable<T> items, params string[] categoryNames)
        where T : IItem => OfCategory(items, (IEnumerable<string>)categoryNames);

    /// <summary>
    /// Selects items not belonging to the specified category.
    /// </summary>
    public static IEnumerable<T> NotOfCategory<T>(this IEnumerable<T> items, FurniCategory category)
        where T : IItem
    {
        return items.Where(item => item.GetCategory() != category);
    }

    /// <summary>
    /// Selects items not belonging to any of the specified categories.
    /// </summary>
    public static IEnumerable<T> NotOfCategory<T>(this IEnumerable<T> items, IEnumerable<FurniCategory> categories)
        where T : IItem
    {
        HashSet<FurniCategory> set = new(categories);
        return items.Where(item => !set.Contains(item.GetCategory()));
    }

    /// <inheritdoc cref="NotOfCategory{T}(IEnumerable{T}, IEnumerable{FurniCategory})"/>
    public static IEnumerable<T> NotOfCategory<T>(this IEnumerable<T> items, params FurniCategory[] categories)
        where T : IItem => NotOfCategory(items, (IEnumerable<FurniCategory>)categories);

    /// <summary>
    /// Selects items without the specified category name.
    /// </summary>
    public static IEnumerable<T> NotOfCategory<T>(this IEnumerable<T> items, string categoryName)
        where T : IItem
    {
        return items.Where(item => !item.GetCategoryName().Equals(categoryName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Selects items without any of the specified category names.
    /// </summary>
    public static IEnumerable<T> NotOfCategory<T>(this IEnumerable<T> items, IEnumerable<string> categoryNames)
        where T : IItem
    {
        HashSet<string> set = new(categoryNames, StringComparer.OrdinalIgnoreCase);
        return items.Where(item => !set.Contains(item.GetCategoryName()));
    }

    /// <inheritdoc cref="NotOfCategory{T}(IEnumerable{T}, IEnumerable{string})"/>
    public static IEnumerable<T> NotOfCategory<T>(this IEnumerable<T> items, params string[] categoryNames)
        where T : IItem => NotOfCategory(items, (IEnumerable<string>)categoryNames);

    /// <summary>
    /// Selects items belonging to the specified furni line.
    /// </summary>
    public static IEnumerable<T> OfLine<T>(this IEnumerable<T> items, string line)
        where T : IItem
    {
        return items.Where(item => item.GetLine().Equals(line, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Selects items belonging to any of the specified furni lines.
    /// </summary>
    public static IEnumerable<T> OfLine<T>(this IEnumerable<T> items, IEnumerable<string> lines)
        where T : IItem
    {
        HashSet<string> set = new(lines, StringComparer.OrdinalIgnoreCase);
        return items.Where(item => set.Contains(item.GetLine()));
    }

    /// <inheritdoc cref="OfLine{T}(IEnumerable{T}, IEnumerable{string})"/>
    public static IEnumerable<T> OfLine<T>(this IEnumerable<T> items, params string[] lines)
        where T : IItem => OfLine(items, (IEnumerable<string>)lines);

    /// <summary>
    /// Selects items not belonging to the specified furni line.
    /// </summary>
    public static IEnumerable<T> NotOfLine<T>(this IEnumerable<T> items, string line)
        where T : IItem
    {
        return items.Where(item => !item.GetLine().Equals(line, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Selects items not belonging to any of the specified furni lines.
    /// </summary>
    public static IEnumerable<T> NotOfLine<T>(this IEnumerable<T> items, IEnumerable<string> lines)
        where T : IItem
    {
        HashSet<string> set = new(lines, StringComparer.OrdinalIgnoreCase);
        return items.Where(item => !set.Contains(item.GetLine()));
    }

    /// <inheritdoc cref="NotOfLine{T}(IEnumerable{T}, IEnumerable{string})"/>
    public static IEnumerable<T> NotOfLine<T>(this IEnumerable<T> items, params string[] lines)
        where T : IItem => NotOfLine(items, (IEnumerable<string>)lines);

    /// <summary>
    /// Selects items with names matching the specified name.
    /// </summary>
    public static IEnumerable<T> Named<T>(this IEnumerable<T> items, string name)
        where T : IItem
    {
        return items.Where(item =>
            TryGetName(item, out string? n) &&
            n.Equals(name, StringComparison.OrdinalIgnoreCase)
        );
    }

    /// <summary>
    /// Selects items with names matching any of the specified names.
    /// </summary>
    public static IEnumerable<T> Named<T>(this IEnumerable<T> items, IEnumerable<string> names)
        where T : IItem
    {
        HashSet<string> set = new(names, StringComparer.OrdinalIgnoreCase);
        return items.Where(item =>
            TryGetName(item, out string? name) &&
            set.Contains(name)
        );
    }

    /// <inheritdoc cref="Named{T}(IEnumerable{T}, IEnumerable{string})"/>
    public static IEnumerable<T> Named<T>(this IEnumerable<T> items, params string[] names)
        where T : IItem => Named(items, (IEnumerable<string>)names);

    /// <summary>
    /// Selects items that do not have the specified name.
    /// </summary>
    public static IEnumerable<T> NotNamed<T>(this IEnumerable<T> items, string name)
        where T : IItem
    {
        return items.Where(item =>
            TryGetName(item, out string? n) &&
            !n.Equals(name, StringComparison.OrdinalIgnoreCase)
        );
    }

    /// <summary>
    /// Selects items that do not have any of the specified names.
    /// </summary>
    public static IEnumerable<T> NotNamed<T>(this IEnumerable<T> items, IEnumerable<string> names)
        where T : IItem
    {
        HashSet<string> set = new(names, StringComparer.OrdinalIgnoreCase);
        return items.Where(item =>
            TryGetName(item, out string? name) &&
            !set.Contains(name)
        );
    }

    /// <inheritdoc cref="NotNamed{T}(IEnumerable{T}, IEnumerable{string})"/>
    public static IEnumerable<T> NotNamed<T>(this IEnumerable<T> items, params string[] names)
        where T : IItem => NotNamed(items, (IEnumerable<string>)names);

    /// <summary>
    /// Selects items with names containing the specified search text.
    /// </summary>
    public static IEnumerable<T> NamedLike<T>(this IEnumerable<T> items, string searchText)
        where T : IItem
    {
        return items.Where(item =>
            TryGetName(item, out string? name) &&
            name.Contains(searchText, StringComparison.OrdinalIgnoreCase)
        );
    }

    /// <summary>
    /// Selects items with names not containing the specified search text.
    /// </summary>
    public static IEnumerable<T> NotNamedLike<T>(this IEnumerable<T> items, string searchText)
        where T : IItem
    {
        return items.Where(item =>
            TryGetName(item, out string? name) &&
            !name.Contains(searchText, StringComparison.OrdinalIgnoreCase)
        );
    }
    #endregion

    #region - Furni -
    public static Point? GetSize<TItem>(this TItem item)
        where TItem : IItem
    {
        if (item is IFloorEntity { Size: Point size })
        {
            return size;
        }

        if (_furniData?.TryGetInfo(item, out var info) == true &&
            info is { Size: Point furniInfoSize })
        {
            return furniInfoSize;
        }

        return null;
    }

    public static bool TryGetSize<TItem>(this TItem item, out Point size)
        where TItem : IItem
    {
        if (item.Type is ItemType.Floor)
        {
            if (item is IFloorItem { Size: Point floorItemSize })
            {
                size = floorItemSize;
                return true;
            }

            if (item is IInventoryItem { Size: Point inventoryItemSize })
            {
                size = inventoryItemSize;
                return true;
            }

            if (item is ITradeItem { Size: Point tradeItemSize })
            {
                size = tradeItemSize;
                return true;
            }

            if (_furniData?.TryGetInfo(item, out var info) == true &&
                info is { Size: Point furniInfoSize })
            {
                size = furniInfoSize;
                return true;
            }
        }

        size = default;
        return false;
    }

    /// <summary>
    /// Gets the area occupied by a floor item.
    /// </summary>
    public static Area? GetArea<TFloorItem>(this TFloorItem item)
        where TFloorItem : IFloorItem
    {
        Point? size = GetSize(item);
        if (size is null)
            return null;
        bool flip = item.Direction % 4 == 2;
        return new(
            item.XY,
            flip ? size.Value.Y : size.Value.X,
            flip ? size.Value.X : size.Value.Y
        );
    }

    /// <summary>
    /// Gets furni that are in the specified state.
    /// </summary>
    public static IEnumerable<TFurni> OfState<TFurni>(this IEnumerable<TFurni> items, int state)
        where TFurni : IFurni
    {
        return items.Where(item => item.State == state);
    }

    /// <inheritdoc cref="OfState{TFurni}(IEnumerable{TFurni}, int)" />
    public static IEnumerable<TFurni> OfState<TFurni, TEnum>(this IEnumerable<TFurni> items, TEnum state)
        where TFurni : IFurni
        where TEnum : unmanaged, Enum
    {
        return OfState(items, state.AsInt32());
    }

    /// <summary>
    /// Gets furni that are in any of the specified states.
    /// </summary>
    public static IEnumerable<TFurni> OfState<TFurni>(this IEnumerable<TFurni> items, IEnumerable<int> states)
        where TFurni : IFurni
    {
        if (states is not HashSet<int> set)
            set = new(states);
        return items.Where(item => set.Contains(item.State));
    }

    /// <inheritdoc cref="OfState{TFurni}(IEnumerable{TFurni}, IEnumerable{int})" />
    public static IEnumerable<TFurni> OfState<TFurni, TState>(this IEnumerable<TFurni> items, IEnumerable<TState> states)
        where TFurni : IFurni
        where TState : unmanaged, Enum
    {
        return OfState(items, states.Select(state => state.AsInt32()));
    }

    /// <inheritdoc cref="OfState{TFurni}(IEnumerable{TFurni}, IEnumerable{int})" />
    public static IEnumerable<TFurni> OfState<TFurni>(this IEnumerable<TFurni> items, params int[] states)
        where TFurni : IFurni => OfState(items, (IEnumerable<int>)states);

    /// <inheritdoc cref="OfState{TFurni}(IEnumerable{TFurni}, IEnumerable{int})" />
    public static IEnumerable<TFurni> OfState<TFurni, TState>(this IEnumerable<TFurni> items, params TState[] states)
        where TFurni : IFurni
        where TState : unmanaged, Enum
    {
        return OfState(items, (IEnumerable<TState>)states);
    }

    /// <summary>
    /// Gets furni that are not in the specified state.
    /// </summary>
    public static IEnumerable<TFurni> NotOfState<TFurni>(this IEnumerable<TFurni> items, int state)
        where TFurni : IFurni
    {
        return items.Where(item => item.State != state);
    }

    /// <inheritdoc cref="NotOfState{TFurni}(IEnumerable{TFurni}, int)" />
    public static IEnumerable<TFurni> NotOfState<TFurni, TState>(this IEnumerable<TFurni> items, TState state)
        where TFurni : IFurni
        where TState : unmanaged, Enum
    {
        return NotOfState(items, state.AsInt32());
    }

    /// <summary>
    /// Gets furni that are not in any of the specified states.
    /// </summary>
    public static IEnumerable<TFurni> NotOfState<TFurni>(this IEnumerable<TFurni> items, IEnumerable<int> states)
        where TFurni : IFurni
    {
        if (states is not HashSet<int> set)
            set = new(states);
        return items.Where(item => !set.Contains(item.State));
    }

    /// <inheritdoc cref="NotOfState{TFurni}(IEnumerable{TFurni}, IEnumerable{int})" />
    public static IEnumerable<TFurni> NotOfState<TFurni, TState>(this IEnumerable<TFurni> items, IEnumerable<TState> states)
        where TFurni : IFurni
        where TState : unmanaged, Enum
    {
        return NotOfState(items, states.Select(state => state.AsInt32()));
    }

    /// <inheritdoc cref="NotOfState{TFurni}(IEnumerable{TFurni}, IEnumerable{int})" />
    public static IEnumerable<TFurni> NotOfState<TFurni>(this IEnumerable<TFurni> items, params int[] states)
        where TFurni : IFurni => NotOfState(items, (IEnumerable<int>)states);

    /// <inheritdoc cref="NotOfState{TFurni}(IEnumerable{TFurni}, IEnumerable{int})" />
    public static IEnumerable<TFurni> NotOfState<TFurni, TState>(this IEnumerable<TFurni> items, params TState[] states)
        where TFurni : IFurni
        where TState : unmanaged, Enum
    {
        return NotOfState(items, (IEnumerable<TState>)states);
    }

    /// <summary>
    /// Gets floor items intersecting the specified area.
    /// Includes items contained inside, and items partially intersecting the area.
    /// </summary>
    public static IEnumerable<TFloorItem> Intersecting<TFloorItem>(this IEnumerable<TFloorItem> items, Area? area)
        where TFloorItem : IFloorItem
    {
        return items.Where(item => GetArea(item)?.Intersects(area) ?? false);
    }

    /// <summary>
    /// Gets items of the specified kind.
    /// </summary>
    public static IEnumerable<T> OfKind<T>(this IEnumerable<T> items, int kind)
        where T : IFloorItem, IWallItem => items.Where(item => item.Kind == kind);

    /// <summary>
    /// Gets items of the specified kinds.
    /// </summary>
    public static IEnumerable<T> OfKinds<T>(this IEnumerable<T> items, IEnumerable<int> kinds)
        where T : IFloorItem, IWallItem
    {
        HashSet<int> set = new(kinds);
        return items.Where(item => set.Contains(item.Kind));
    }

    /// <summary>
    /// Gets items of the specified kinds.
    /// </summary>
    public static IEnumerable<T> OfKinds<T>(this IEnumerable<T> items, params int[] kinds)
        where T : IFloorItem, IWallItem => OfKinds<T>(items, (IEnumerable<int>)kinds);

    /// <summary>
    /// Gets items owned by the specified user ID.
    /// </summary>
    public static IEnumerable<T> OwnedBy<T>(this IEnumerable<T> items, Id ownerId)
        where T : IFurni => items.Where(item => item.OwnerId == ownerId);

    /// <summary>
    /// Gets items owned by the specified user.
    /// </summary>
    public static IEnumerable<T> OwnedBy<T>(this IEnumerable<T> items, string ownerName) where T : IFurni
        => items.Where(item => string.Equals(item.OwnerName, ownerName, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Gets wall items placed at the specified location.
    /// </summary>
    public static IEnumerable<TWallItem> At<TWallItem>(this IEnumerable<TWallItem> items,
        int? wx = null, int? wy = null, int? lx = null, int? ly = null,
        WallOrientation? orientation = null) where TWallItem : IWallItem
    {
        foreach (var item in items)
        {
            if (wx.HasValue && item.WX != wx) continue;
            if (wy.HasValue && item.WY != wy) continue;
            if (lx.HasValue && item.LX != lx) continue;
            if (ly.HasValue && item.LY != ly) continue;
            if (orientation.HasValue && item.Orientation != orientation) continue;
            yield return item;
        }
    }

    /// <summary>
    /// Gets wall items placed at the specified location.
    /// </summary>
    public static IEnumerable<TWallItem> At<TWallItem>(this IEnumerable<TWallItem> items, WallLocation location) where TWallItem : IWallItem
        => At(items, location.Wall.X, location.Wall.Y, location.Offset.X, location.Offset.Y, location.Orientation);
    #endregion

    #region - Inventory -
    /// <summary>
    /// Gets tradeable inventory items.
    /// </summary>
    public static IEnumerable<IInventoryItem> GetTradeable(this IEnumerable<IInventoryItem> items)
        => items.Where(item => item.IsTradeable);
    /// <summary>
    /// Gets sellable inventory items.
    /// </summary>
    public static IEnumerable<IInventoryItem> GetSellable(this IEnumerable<IInventoryItem> items)
        => items.Where(item => item.IsSellable);

    /// <summary>
    /// Splits inventory items into groups limited by the specified maximum slot and item counts.
    /// </summary>
    public static IEnumerable<IGrouping<int, IInventoryItem>> Group(this IEnumerable<IInventoryItem> items, int maxSlots = 9, int maxItems = 1500)
    {
        if (maxSlots < 1) throw new ArgumentOutOfRangeException(nameof(maxSlots));
        if (maxItems < 1) throw new ArgumentOutOfRangeException(nameof(maxItems));

        int groupIndex = 0, currentSlots = 0, currentCount = 0;
        var lastKind = ((ItemType)(-1), -1);

        return items
            .OrderBy(item => item.Type)
            .ThenBy(item => item.Kind)
            .GroupBy(item =>
            {
                var kind = (item.Type, item.Kind);
                if (!item.IsGroupable || lastKind != kind)
                    currentSlots++;
                currentCount++;
                lastKind = kind;

                if (currentSlots > maxSlots || currentCount > maxItems)
                {
                    currentCount = 0;
                    currentSlots = 1;
                    return ++groupIndex;
                }

                return groupIndex;
            });
    }

    /// <summary>
    /// Groups the specified inventory items into fragments to be sent to the client.
    /// </summary>
    /// <param name="items">The items to group into fragments.</param>
    /// <param name="size">The maximum number of items per fragment.</param>
    public static IEnumerable<InventoryFragment> Fragmentize(this IEnumerable<IInventoryItem> items, int size = 600)
    {
        if (items is not IInventoryItem[] array)
        {
            array = items.ToArray();
        }

        int currentIndex = 0;
        int totalFragments = (int)Math.Ceiling(array.Length / (double)size);

        foreach (var group in array.GroupBy(x => currentIndex++ / size))
        {
            yield return new InventoryFragment(group)
            {
                Index = group.Key,
                Total = totalFragments
            };
        }
    }
    #endregion

    #region - Rooms -
    public static IEnumerable<IRoomInfo> Filter(
        this IEnumerable<IRoomInfo> rooms,
        string? name = null,
        string? description = null,
        int? ownerId = null,
        string? owner = null,
        RoomAccess? access = null,
        TradePermissions? trading = null,
        RoomCategory? category = null,
        int? groupId = null,
        string? group = null)
    {
        foreach (var roomInfo in rooms)
        {
            if (name != null && !roomInfo.Name.ToLower().Contains(name.ToLower())) continue;
            if (description != null && !roomInfo.Description.ToLower().Contains(description.ToLower())) continue;
            if (ownerId.HasValue && roomInfo.OwnerId != ownerId) continue;
            if (owner != null && !roomInfo.OwnerName.Equals(owner, StringComparison.InvariantCultureIgnoreCase)) continue;
            if (access.HasValue && roomInfo.Access != access) continue;
            if (trading.HasValue && roomInfo.Trading != trading) continue;
            if (category.HasValue && roomInfo.Category != category) continue;
            if (groupId.HasValue && (!roomInfo.IsGroupRoom || roomInfo.GroupId != groupId)) continue;
            if (group != null && (!roomInfo.IsGroupRoom || !roomInfo.GroupName.ToLower().Contains(group.ToLower()))) continue;
            yield return roomInfo;
        }
    }
    #endregion

    #region - Avatars -
    /// <summary>
    /// Gets room avatars at the specified X, Y, Z location and/or direction.
    /// </summary>
    public static IEnumerable<T> At<T>(this IEnumerable<T> avatars,
        int? x = null, int? y = null, float? z = null, int? dir = null,
        float epsilon = XabboConst.DefaultEpsilon) where T : IFloorEntity
    {
        foreach (var e in avatars)
        {
            if (x.HasValue && e.Location.X != x) continue;
            if (y.HasValue && e.Location.Y != y) continue;
            if (z.HasValue && Math.Abs(e.Location.Z - z.Value) >= epsilon) continue;
            if (dir.HasValue && e.Direction != dir.Value) continue;
            yield return e;
        }
    }

    /// <summary>
    /// Gets room avatars not a the specified X, Y, Z location and/or direction.
    /// </summary>
    public static IEnumerable<T> NotAt<T>(this IEnumerable<T> avatars,
        int? x = null, int? y = null, float? z = null, int? dir = null,
        float epsilon = XabboConst.DefaultEpsilon) where T : IFloorEntity
    {
        foreach (var e in avatars)
        {
            if (x.HasValue && e.Location.X == x) continue;
            if (y.HasValue && e.Location.Y == y) continue;
            if (z.HasValue && Math.Abs(e.Location.Z - z.Value) < epsilon) continue;
            if (dir.HasValue && e.Direction == dir) continue;
            yield return e;
        }
    }

    /// <summary>
    /// Gets floor entities at the specified X, Y location and optionally direction.
    /// </summary>
    public static IEnumerable<TFloorAvatar> At<TFloorAvatar>(this IEnumerable<TFloorAvatar> avatars,
        Point location, int? dir = null) where TFloorAvatar : IFloorEntity
    {
        return At(avatars, location.X, location.Y, dir: dir);
    }

    /// <summary>
    /// Gets floor entities not at the specified X, Y location and optionally direction.
    /// </summary>
    public static IEnumerable<TFloorAvatar> NotAt<TFloorAvatar>(this IEnumerable<TFloorAvatar> avatars,
        Point location, int? dir = null) where TFloorAvatar : IFloorEntity
    {
        return NotAt(avatars, location.X, location.Y, dir: dir);
    }

    /// <summary>
    /// Gets floor entities at the specified X, Y, Z location and optionally direction.
    /// </summary>
    public static IEnumerable<TFloorAvatar> At<TFloorAvatar>(this IEnumerable<TFloorAvatar> avatars,
        Tile location, int? dir = null,
        float epsilon = XabboConst.DefaultEpsilon) where TFloorAvatar : IFloorEntity
    {
        return At(avatars, location.X, location.Y, location.Z, dir, epsilon);
    }

    /// <summary>
    /// Gets floor entities not at the specified X, Y, Z location and optionally direction.
    /// </summary>
    public static IEnumerable<TFloorAvatar> NotAt<TFloorAvatar>(this IEnumerable<TFloorAvatar> avatars,
        Tile location, int? dir = null,
        float epsilon = XabboConst.DefaultEpsilon) where TFloorAvatar : IFloorEntity
    {
        return NotAt(avatars, location.X, location.Y, location.Z, dir, epsilon);
    }

    /// <summary>
    /// Gets floor entities contained inside the specified area.
    /// Entities partially intersecting the area are not considered to be inside.
    /// To include these, use <see cref="Intersecting{TFloorItem}(IEnumerable{TFloorItem}, Area?)" />.
    /// </summary>
    public static IEnumerable<TFloorAvatar> Inside<TFloorAvatar>(this IEnumerable<TFloorAvatar> avatars, Area area)
        where TFloorAvatar : IFloorEntity
    {
        return avatars.Where(x => area.Contains(x.Area));
    }

    /// <summary>
    /// Gets floor entities contained inside any of the areas in the specified set.
    /// </summary>
    public static IEnumerable<TFloorAvatar> InsideAny<TFloorAvatar>(this IEnumerable<TFloorAvatar> avatars, AreaSet areas)
        where TFloorAvatar : IFloorEntity
    {
        return avatars.Where(x => x.Area is { } area && areas.Any(area => area.Contains(area)));
    }

    /// <summary>
    /// Gets floor entities contained inside all of the areas in the specified set.
    /// </summary>
    public static IEnumerable<TFloorAvatar> InsideAll<TFloorAvatar>(this IEnumerable<TFloorAvatar> avatars, AreaSet areas)
        where TFloorAvatar : IFloorEntity
    {
        return avatars.Where(x => x.Area is { } area && areas.All(area => area.Contains(x.Area)));
    }

    /// <summary>
    /// Gets floor entities outside the specified area.
    /// Entities partially intersecting the area are not considered to be outside.
    /// </summary>
    public static IEnumerable<TFloorAvatar> Outside<TFloorAvatar>(this IEnumerable<TFloorAvatar> avatars, Area area)
        where TFloorAvatar : IFloorEntity
    {
        return avatars.Where(x => !area.Contains(x.Area));
    }

    /// <summary>
    /// Gets floor entities outside the specified areas.
    /// Entities partially intersecting any of the areas are not considered to be outside.
    /// </summary>
    public static IEnumerable<TFloorAvatar> Outside<TFloorAvatar>(this IEnumerable<TFloorAvatar> avatars, AreaSet areas)
        where TFloorAvatar : IFloorEntity
    {
        return avatars.Where(x => x.Area is { } area && !areas.Any(area => area.Intersects(area)));
    }
    #endregion
}
