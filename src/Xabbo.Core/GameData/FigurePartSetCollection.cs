using System.Collections.Immutable;
using System.Linq;

namespace Xabbo.Core.GameData;

/// <summary>
/// Defines a collection of figure part sets of the same <see cref="FigurePartType"/>.
/// </summary>
public sealed record FigurePartSetCollection(
    FigurePartType Type,
    int PaletteId,
    ImmutableDictionary<int, FigurePartSet> PartSets,
    int Mand_m_0 = 0,
    int Mand_f_0 = 0,
    int Mand_m_1 = 0,
    int Mand_f_1 = 0
)
{
    internal FigurePartSetCollection(FigurePartType partSetType, Json.Origins.FigureData figureData) : this(
        Type: partSetType,
        PaletteId: -1,
        PartSets: figureData.MalePartSets[partSetType.ToShortString()]
            .Select(partSet => new FigurePartSet(Gender.Male, partSet))
            .Concat(
                figureData.FemalePartSets[partSetType.ToShortString()]
                .Select(partSet => new FigurePartSet(Gender.Female, partSet))
            )
            .ToImmutableDictionary(partSet => partSet.Id)
    )
    { }

    internal FigurePartSetCollection(Xml.FigureData.PartSetCollection proxy) : this(
        Type: H.GetFigurePartType(proxy.Type),
        PaletteId: proxy.PaletteId,
        Mand_m_0: proxy.mand_m_0,
        Mand_f_0: proxy.mand_f_0,
        Mand_m_1: proxy.mand_m_1,
        Mand_f_1: proxy.mand_f_1,
        PartSets: proxy.Sets
            .Select(partSet => new FigurePartSet(partSet))
            .ToImmutableDictionary(partSet => partSet.Id)
    )
    { }
}
