using System.Collections.Immutable;

namespace Xabbo.Core.GameData;

/// <summary>
/// Contains information of a furni.
/// </summary>
public sealed record FurniInfo(
    ItemType Type,
    int Kind,
    string Identifier,
    int Revision = 0,
    int DefaultDirection = 0,
    int XDimension = 0,
    int YDimension = 0,
    ImmutableArray<string> PartColors = default,
    string Name = "",
    string Description = "",
    string AdUrl = "",
    int OfferId = 0,
    bool BuyOut = false,
    int RentOfferId = 0,
    bool RentBuyOut = false,
    bool IsBuildersClub = false,
    bool ExcludedDynamic = false,
    string CustomParams = "",
    FurniCategory Category = FurniCategory.Unknown,
    string CategoryName = "",
    bool CanStandOn = false,
    bool CanSitOn = false,
    bool CanLayOn = false,
    string Line = "",
    string Environment = "",
    bool IsRare = false
)
{
    public ImmutableArray<string> PartColors { get; } = PartColors.IsDefault ? [] : PartColors;

    internal FurniInfo(ItemType type, Xml.FurniInfo proxy) : this(
        Type: type,
        Kind: proxy.Id,
        Identifier: proxy.ClassName ?? "",
        Revision: proxy.Revision,
        DefaultDirection: proxy.DefaultDirection,
        XDimension: proxy.XDimension,
        YDimension: proxy.YDimension,
        PartColors: [.. proxy.PartColors],
        Name: proxy.Name ?? "",
        Description: proxy.Description ?? "",
        AdUrl: proxy.AdUrl ?? "",
        OfferId: proxy.OfferId,
        BuyOut: proxy.BuyOut,
        RentOfferId: proxy.RentOfferId,
        RentBuyOut: proxy.RentBuyOut,
        IsBuildersClub: proxy.IsBuildersClub,
        ExcludedDynamic: proxy.ExcludedDynamic,
        CustomParams: proxy.CustomParams ?? "",
        Category: (FurniCategory)proxy.SpecialType,
        CanStandOn: proxy.CanStandOn,
        CanSitOn: proxy.CanSitOn,
        CanLayOn: proxy.CanLayOn,
        Line: proxy.FurniLine ?? ""
    )
    { }

    internal FurniInfo(ItemType type, Json.FurniInfo proxy) : this(
        Type: type,
        Kind: proxy.Id,
        Identifier: proxy.ClassName ?? "",
        Revision: proxy.Revision,
        DefaultDirection: proxy.DefaultDir,
        XDimension: proxy.XDim,
        YDimension: proxy.YDim,
        PartColors: proxy.PartColors.Colors.ToImmutableArray(),
        Name: proxy.Name ?? "",
        Description: proxy.Description ?? "",
        AdUrl: proxy.AdUrl ?? "",
        OfferId: proxy.OfferId,
        BuyOut: proxy.Buyout,
        RentOfferId: proxy.RentOfferId,
        RentBuyOut: proxy.RentBuyout,
        IsBuildersClub: proxy.BC,
        ExcludedDynamic: proxy.ExcludedDynamic,
        CustomParams: proxy.CustomParams ?? "",
        Category: (FurniCategory)proxy.SpecialType,
        CategoryName: proxy.Category ?? "",
        CanStandOn: proxy.CanStandOn,
        CanSitOn: proxy.CanSitOn,
        CanLayOn: proxy.CanLayOn,
        Line: proxy.FurniLine ?? "",
        Environment: proxy.Environment ?? "",
        IsRare: proxy.Rare
    )
    { }

    /// <summary>
    /// Returns <c>true</c> if <see cref="CanStandOn"/>, <see cref="CanSitOn"/> and <see cref="CanLayOn"/> are all <c>false</c>.
    /// </summary>
    public bool IsUnwalkable => !CanStandOn && !CanSitOn && !CanLayOn;
}
