using System.Collections.Immutable;
using System.Linq;

namespace Xabbo.Core.GameData;

public sealed class FigurePartSet
{
    public int Id { get; }
    public Gender Gender { get; }
    public int RequiredClubLevel { get; }
    public bool IsColorable { get; }
    public bool IsSelectable { get; }
    public bool IsPreSelectable { get; }
    public bool IsSellable { get; }
    public ImmutableArray<FigurePart> Parts { get; }

    public bool IsClubRequired => RequiredClubLevel > 0;

    internal FigurePartSet(Gender gender, Json.Origins.FigurePartSet partSet)
    {
        Id = partSet.Id;
        Gender = gender;
        Parts = partSet.Parts
            .Select(part => new FigurePart
            {
                Type = H.GetFigurePartType(part.Key),
                Id = part.Value
            })
            .ToImmutableArray();
    }

    internal FigurePartSet(Xml.FigureData.PartSet proxy)
    {
        Id = proxy.Id;
        Gender = H.ToGender(proxy.Gender);
        RequiredClubLevel = proxy.RequiredClubLevel;
        IsColorable = proxy.IsColorable;
        IsSelectable = proxy.IsSelectable;
        IsPreSelectable = proxy.IsPreSelectable;
        IsSellable = proxy.IsSellable;
        Parts = proxy.Parts
            .Select(part => new FigurePart(part))
            .ToImmutableArray();
    }
}