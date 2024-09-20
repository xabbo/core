using System.Collections.Immutable;
using System.Linq;

namespace Xabbo.Core.GameData;

public sealed record FigurePartSet(
    int Id,
    Gender Gender,
    ImmutableArray<FigurePart> Parts,
    int RequiredClubLevel = 0,
    bool IsColorable = false,
    bool IsSelectable = false,
    bool IsPreSelectable = false,
    bool IsSellable = false
)
{
    public bool IsClubRequired => RequiredClubLevel > 0;

    internal FigurePartSet(Gender gender, Json.Origins.FigurePartSet partSet) : this(
        Id: partSet.Id,
        Gender: gender,
        Parts: partSet.Parts
            .Select(part => new FigurePart(H.GetFigurePartType(part.Key), part.Value))
            .ToImmutableArray()
    )
    {
    }

    internal FigurePartSet(Xml.FigureData.PartSet proxy) : this(
        Id: proxy.Id,
        Gender: H.ToGender(proxy.Gender),
        RequiredClubLevel: proxy.RequiredClubLevel,
        IsColorable: proxy.IsColorable,
        IsSelectable: proxy.IsSelectable,
        IsPreSelectable: proxy.IsPreSelectable,
        IsSellable: proxy.IsSellable,
        Parts: proxy.Parts
            .Select(part => new FigurePart(part))
            .ToImmutableArray()
    ) { }
}