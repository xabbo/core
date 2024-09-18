using System.Collections.Immutable;
using System.Linq;

namespace Xabbo.Core.GameData;

public sealed class FigureColorPalette
{
    public int Id { get; init; }
    public ImmutableDictionary<int, FigurePartColor> Colors { get; init; } = ImmutableDictionary<int, FigurePartColor>.Empty;

    public FigureColorPalette() { }

    internal FigureColorPalette(Xml.FigureData.Palette proxy)
    {
        Id = proxy.Id;
        Colors = proxy.Colors
            .Select(color => new FigurePartColor(color))
            .ToImmutableDictionary(color => color.Id);
    }
}