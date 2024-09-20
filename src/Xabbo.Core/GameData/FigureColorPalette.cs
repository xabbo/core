using System.Collections.Immutable;
using System.Linq;

namespace Xabbo.Core.GameData;

public sealed record FigureColorPalette(
    int Id,
    ImmutableDictionary<int, FigurePartColor> Colors
)
{
    internal FigureColorPalette(Xml.FigureData.Palette proxy) : this(
        Id: proxy.Id,
        Colors: proxy.Colors
            .Select(color => new FigurePartColor(color))
            .ToImmutableDictionary(color => color.Id)
    )
    { }
}
