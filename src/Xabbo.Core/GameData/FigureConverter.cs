using System.Collections.Generic;

namespace Xabbo.Core.GameData;

/// <summary>
/// Converts origins figure strings into their modern representation.
/// </summary>
public sealed class FigureConverter
{
    private static readonly Dictionary<int, int> _hairToHatMap = new()
    {
        // m
        [120] = 1001,
        [130] = 1010,
        [140] = 1004,
        [150] = 1003,
        [160] = 1004,
        [175] = 1006,
        [176] = 1007,
        [177] = 1008,
        [178] = 1009,
        [800] = 1012,
        [801] = 1011,
        [802] = 1013,
        // f
        [525] = 1002,
        [535] = 1003,
        [565] = 1004,
        [570] = 1005,
        [580] = 1007,
        [585] = 1006,
        [590] = 1008,
        [595] = 1009,
        [810] = 1012,
        [811] = 1013,
    };

    public FigureData ModernFigureData { get; }
    public FigureData OriginsFigureData { get; }

    // Maps PaletteId/Color -> ColorId.
    private readonly Dictionary<(int PaletteId, string Color), int> _colorMap = [];

    // Maps Origins FigurePartSet.Id -> FigurePartType.
    private readonly Dictionary<int, FigurePartType> _partSetTypeMap = [];

    /// <summary>
    /// Creates a new figure converter with the specified origins and modern figure data.
    /// </summary>
    /// <param name="modernFigureData">The modern figure data.</param>
    /// <param name="originsFigureData">The origins figure data.</param>
    public FigureConverter(FigureData modernFigureData, FigureData originsFigureData)
    {
        ModernFigureData = modernFigureData;
        OriginsFigureData = originsFigureData;

        foreach (var (paletteId, palette) in ModernFigureData.Palettes)
        {
            foreach (var color in palette.Colors.Values)
            {
                var key = (paletteId, color.Value.ToLower());
                _colorMap.TryAdd(key, color.Id);
            }
        }

        foreach (var partSetCollection in originsFigureData.SetCollections.Values)
        {
            foreach (var partSet in partSetCollection.PartSets.Values)
            {
                _partSetTypeMap[partSet.Id] = partSetCollection.Type;
            }
        }
    }

    /// <summary>
    /// Converts the specified origins figure string to its modern <see cref="Figure"/> representation.
    /// </summary>
    public Figure ToModern(string originsFigureString)
    {
        Figure figure = [];
        for (int i = 0; i < originsFigureString.Length; i += 5)
        {
            int id = int.Parse(originsFigureString[i..(i + 3)]);
            int colorIndex = int.Parse(originsFigureString[(i + 3)..(i + 5)]);

            FigurePartType partType = _partSetTypeMap[id];
            int paletteId = ModernFigureData.SetCollections[partType].PaletteId;

            var color = OriginsFigureData.Palettes[id].Colors[colorIndex - 1];
            if (_colorMap.TryGetValue((paletteId, color.Value.ToLower()), out int colorId))
                figure.Add(partType, id, colorId);
            else
                figure.Add(partType, id);

            if (_hairToHatMap.TryGetValue(id, out int hatId))
                figure.Add(FigurePartType.Hat, hatId, [.. figure[partType].Colors]);
        }
        return figure;
    }
}
