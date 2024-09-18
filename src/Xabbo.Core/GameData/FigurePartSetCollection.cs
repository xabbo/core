using System.Collections.Immutable;
using System.Linq;

namespace Xabbo.Core.GameData;

public sealed class FigurePartSetCollection
{
    /// <summary>
    /// The figure part type of this part set collection.
    /// </summary>
    public FigurePartType Type { get; init; }

    /// <summary>
    /// The color palette that this part set collection uses.
    /// </summary>
    public int PaletteId { get; init; }

    public int Mand_m_0 { get; init; }
    public int Mand_f_0 { get; init; }
    public int Mand_m_1 { get; init; }
    public int Mand_f_1 { get; init; }

    public ImmutableDictionary<int, FigurePartSet> PartSets { get; init; } = ImmutableDictionary<int, FigurePartSet>.Empty;

    public FigurePartSetCollection() { }

    internal FigurePartSetCollection(FigurePartType partSetType, Json.Origins.FigureData figureData)
    {
        PaletteId = -1;

        Type = partSetType;

        PartSets = figureData.MalePartSets[partSetType.ToShortString()]
            .Select(partSet => new FigurePartSet(Gender.Male, partSet))
            .Concat(
                figureData.FemalePartSets[partSetType.ToShortString()]
                .Select(partSet => new FigurePartSet(Gender.Female, partSet))
            )
            .ToImmutableDictionary(partSet => partSet.Id);
    }

    internal FigurePartSetCollection(Xml.FigureData.PartSetCollection proxy)
    {
        Type = H.GetFigurePartType(proxy.Type);
        PaletteId = proxy.PaletteId;
        Mand_m_0 = proxy.mand_m_0;
        Mand_f_0 = proxy.mand_f_0;
        Mand_m_1 = proxy.mand_m_1;
        Mand_f_1 = proxy.mand_f_1;
        PartSets = proxy.Sets
            .Select(partSet => new FigurePartSet(partSet))
            .ToImmutableDictionary(partSet => partSet.Id);
    }
}