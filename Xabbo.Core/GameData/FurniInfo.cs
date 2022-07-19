using System;
using System.Collections.Immutable;

namespace Xabbo.Core.GameData;

/// <summary>
/// Contains information of a furni.
/// </summary>
public record FurniInfo : IItem
{
    long IItem.Id => -1;

    /// <summary>
    /// Gets the type of the furni.
    /// </summary>
    public ItemType Type { get; init; }
    /// <summary>
    /// Gets the kind of the furni.
    /// </summary>
    public int Kind { get; init; }
    /// <summary>
    /// Gets the unique identifier string of the furni.
    /// </summary>
    public string Identifier { get; init; } = string.Empty;
    public int Revision { get; init; }
    public int DefaultDirection { get; init; }
    public int XDimension { get; init; }
    public int YDimension { get; init; }
    public ImmutableArray<string> PartColors { get; init; }
    /// <summary>
    /// Gets the name of the furni.
    /// </summary>
    public string Name { get; init; } = string.Empty;
    /// <summary>
    /// Gets the description of the furni.
    /// </summary>
    public string Description { get; init; } = string.Empty;
    public string AdUrl { get; init; } = string.Empty;
    public int OfferId { get; init; }
    public bool BuyOut { get; init; }
    public int RentOfferId { get; init; }
    public bool RentBuyOut { get; init; }
    public bool IsBuildersClub { get; init; }
    public bool ExcludedDynamic { get; init; }
    public string CustomParams { get; init; }
    /// <summary>
    /// Gets the category of the furni.
    /// </summary>
    public FurniCategory Category { get; init; }
    /// <summary>
    /// Gets the category name of the furni.
    /// </summary>
    public string CategoryName { get; init; } = string.Empty;
    public bool CanStandOn { get; init; }
    public bool CanSitOn { get; init; }
    public bool CanLayOn { get; init; }
    public string Line { get; init; } = string.Empty;
    public string Environment { get; init; } = string.Empty;
    public bool IsRare { get; init; }

    internal FurniInfo(ItemType type, Xml.FurniInfo proxy)
    {
        Type = type;
        Kind = proxy.Id;
        Identifier = proxy.ClassName ?? string.Empty;
        Revision = proxy.Revision;
        DefaultDirection = proxy.DefaultDirection;
        XDimension = proxy.XDimension;
        YDimension = proxy.YDimension;
        PartColors = proxy.PartColors.ToImmutableArray();
        Name = proxy.Name ?? string.Empty;
        Description = proxy.Description ?? string.Empty;
        AdUrl = proxy.AdUrl ?? string.Empty;
        OfferId = proxy.OfferId;
        BuyOut = proxy.BuyOut;
        RentOfferId = proxy.RentOfferId;
        RentBuyOut = proxy.RentBuyOut;
        IsBuildersClub = proxy.IsBuildersClub;
        ExcludedDynamic = proxy.ExcludedDynamic;
        CustomParams = proxy.CustomParams ?? string.Empty;
        Category = (FurniCategory)proxy.SpecialType;
        CanStandOn = proxy.CanStandOn;
        CanSitOn = proxy.CanSitOn;
        CanLayOn = proxy.CanLayOn;
        Line = proxy.FurniLine ?? string.Empty;
    }

    internal FurniInfo(ItemType type, Json.FurniInfo proxy)
    {
        Type = type;
        Kind = proxy.Id;
        Identifier = proxy.ClassName ?? string.Empty;
        Revision = proxy.Revision;
        DefaultDirection = proxy.DefaultDir;
        XDimension = proxy.XDim;
        YDimension = proxy.YDim;
        PartColors = proxy.PartColors.Colors.ToImmutableArray();
        Name = proxy.Name ?? string.Empty;
        Description = proxy.Description ?? string.Empty;
        AdUrl = proxy.AdUrl ?? string.Empty;
        OfferId = proxy.OfferId;
        BuyOut = proxy.Buyout;
        RentOfferId = proxy.RentOfferId;
        RentBuyOut = proxy.RentBuyout;
        IsBuildersClub = proxy.BC;
        ExcludedDynamic = proxy.ExcludedDynamic;
        CustomParams = proxy.CustomParams ?? string.Empty;
        Category = (FurniCategory)proxy.SpecialType;
        CategoryName = proxy.Category ?? string.Empty;
        CanStandOn = proxy.CanStandOn;
        CanSitOn = proxy.CanSitOn;
        CanLayOn = proxy.CanLayOn;
        Line = proxy.FurniLine ?? string.Empty;
        Environment = proxy.Environment ?? string.Empty;
        IsRare = proxy.Rare;
    }

    /// <summary>
    /// Returns <c>true</c> if <see cref="CanStandOn"/>, <see cref="CanSitOn"/> and <see cref="CanLayOn"/> are all <c>false</c>.
    /// </summary>
    public bool IsUnwalkable => !CanStandOn && !CanSitOn && !CanLayOn;
}
