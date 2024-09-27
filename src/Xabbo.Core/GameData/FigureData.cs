using System.IO;
using System.Linq;
using System.Collections.Immutable;

namespace Xabbo.Core.GameData;

/// <summary>
/// Defines a map of <see cref="FigurePartSetCollection"/> and <see cref="FigureColorPalette"/>.
/// </summary>
public sealed class FigureData
{
    /// <summary>
    /// Loads figure data in modern XML format from the specified stream.
    /// </summary>
    public static FigureData LoadXml(Stream stream) => new(Xml.FigureData.Load(stream));

    /// <summary>
    /// Loads figure data in modern XML format from the specified file path.
    /// </summary>
    public static FigureData LoadXml(string path)
    {
        using var stream = File.OpenRead(path);
        return LoadXml(stream);
    }

    /// <summary>
    /// Loads figure data in origins JSON format from the specified file path.
    /// </summary>
    public static FigureData LoadJsonOrigins(string filePath) => new(Json.Origins.FigureData.Load(filePath));

    /// <summary>
    /// Defines a map of ID to <see cref="FigureColorPalette"/>.
    /// </summary>
    public ImmutableDictionary<int, FigureColorPalette> Palettes { get; }

    /// <summary>
    /// Defines a map of <see cref="FigurePartType"/> to <see cref="FigurePartSetCollection"/>.
    /// </summary>
    public ImmutableDictionary<FigurePartType, FigurePartSetCollection> SetCollections { get; }

    /// <summary>
    /// Gets the palette for the specified <see cref="FigurePartSetCollection"/>.
    /// </summary>
    public FigureColorPalette GetPalette(FigurePartSetCollection setCollection) => Palettes[setCollection.PaletteId];

    /// <summary>
    /// Gets the palette for the specified <see cref="FigurePartType"/>.
    /// </summary>
    public FigureColorPalette GetPalette(FigurePartType figurePartType) => Palettes[SetCollections[figurePartType].PaletteId];

    internal FigureData(Xml.FigureData proxy)
    {
        Palettes = proxy.Palettes
            .Select(palette => new FigureColorPalette(palette))
            .ToImmutableDictionary(palette => palette.Id);

        SetCollections = proxy.SetCollections
            .Select(setCollection => new FigurePartSetCollection(setCollection))
            .ToImmutableDictionary(setCollection => setCollection.Type);
    }

    internal FigureData(Json.Origins.FigureData proxy)
    {
        // Each figure part set in the Origins figure data has its own list of colors.
        // This means we cannot use the PartSetType.PaletteId to reference a color palette.
        // Therefore, we generate a color palette for each figure part set as they have unique IDs,
        // using the figure part set ID as the color palette ID,
        // and the index into each figure part set's color list as the color's ID.
        Palettes = proxy.MalePartSets.Values.Concat(proxy.FemalePartSets.Values)
            .SelectMany(partSets => partSets)
            .ToImmutableDictionary(
                partSet => partSet.Id,
                partSet => new FigureColorPalette(
                    Id: partSet.Id,
                    Colors: partSet.Colors
                        .Select((color, index) => (color, index))
                        .ToImmutableDictionary(
                            pair => pair.index,
                            pair => new FigurePartColor(pair.index, pair.index, pair.color)
                        )
                )
            );

        SetCollections = proxy.MalePartSets.Keys.Concat(proxy.FemalePartSets.Keys)
            .Distinct()
            .ToImmutableDictionary(
                H.GetFigurePartType,
                partSetType => new FigurePartSetCollection(
                    H.GetFigurePartType(partSetType), proxy)
            );
    }

    /// <summary>
    /// Attempts to derive the specified Figure's gender using information from the figure data.
    /// </summary>
    public bool TryGetGender(Figure figure, out Gender gender)
    {
        gender = Gender.Unisex;

        foreach (var part in figure.Parts)
        {
            if (!SetCollections.TryGetValue(part.Type, out FigurePartSetCollection? partSetCollection))
                continue;
            if (!partSetCollection.PartSets.TryGetValue(part.Id, out FigurePartSet? partSet))
                continue;
            if (partSet.Gender != Gender.Unisex)
            {
                gender = partSet.Gender;
                break;
            }
        }

        return gender != Gender.Unisex;
    }

    /// <summary>
    /// Attempts to derive the specified Figure's gender using information from the figure data.
    /// </summary>
    public bool TryGetGender(string figureString, out Gender gender)
    {
        gender = Gender.Unisex;
        if (!Figure.TryParse(figureString, out Figure? figure))
            return false;
        return TryGetGender(figure, out gender);
    }
}
