using System.Collections.Immutable;

namespace Xabbo.Core.GameData;

/// <summary>
/// Defines information about a furniture.
/// </summary>
/// <param name="Type">The furniture type.</param>
/// <param name="Kind">The furniture kind.</param>
/// <param name="Identifier">The furniture's unique string identifier.</param>
/// <param name="Revision">The revision number.</param>
/// <param name="DefaultDirection">The default direction when placed in a room.</param>
/// <param name="Size">The size of the furniture.</param>
/// <param name="PartColors"></param>
/// <param name="Name">The name of the furniture.</param>
/// <param name="Description">The description of the furniture.</param>
/// <param name="AdUrl"></param>
/// <param name="OfferId"></param>
/// <param name="BuyOut"></param>
/// <param name="RentOfferId"></param>
/// <param name="RentBuyOut"></param>
/// <param name="IsBuildersClub"></param>
/// <param name="ExcludedDynamic"></param>
/// <param name="CustomParams"></param>
/// <param name="Category"></param>
/// <param name="CategoryName"></param>
/// <param name="CanStandOn"></param>
/// <param name="CanSitOn"></param>
/// <param name="CanLayOn"></param>
/// <param name="Line"></param>
/// <param name="Environment"></param>
/// <param name="IsRare"></param>
/// <remarks>
/// <see cref="Kind"/> is the numeric identifier of a furniture.
/// This number can be different across hotels.
/// It is only unique for each item type - a floor and a wall item may have the same kind.
/// <para/>
/// <see cref="Identifier"/> is a unique string identifier, also known as its <i>class name</i>.
/// This identifier is unique across both furniture types, and is the same across hotels.
/// </remarks>
public sealed record FurniInfo(

    ItemType Type,
    int Kind,
    string Identifier,
    int Revision = 0,
    int DefaultDirection = 0,
    Point? Size = null,
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
        Size: type is ItemType.Floor ? (proxy.XDimension, proxy.YDimension) : null,
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
        Size: type is ItemType.Floor ? (proxy.XDim, proxy.YDim) : null,
        PartColors: [.. proxy.PartColors.Colors],
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

    internal FurniInfo(Json.Origins.FurniInfo proxy) : this(
        Type: proxy.Type,
        Kind: proxy.Kind,
        Identifier: proxy.Identifier,
        Revision: proxy.Revision,
        DefaultDirection: proxy.DefaultDir,
        Size: proxy.Type is ItemType.Floor ? (proxy.XDim, proxy.YDim) : null,
        PartColors: [.. proxy.PartColors.Split(',')],
        Name: proxy.Name,
        Description: proxy.Description,
        AdUrl: proxy.AdUrl,
        CustomParams: proxy.CustomParams,
        Category: (FurniCategory)proxy.SpecialType
    )
    { }

    /// <summary>
    /// Returns <c>true</c> if <see cref="CanStandOn"/>, <see cref="CanSitOn"/> and <see cref="CanLayOn"/> are all <c>false</c>.
    /// </summary>
    public bool IsUnwalkable => !CanStandOn && !CanSitOn && !CanLayOn;
}
