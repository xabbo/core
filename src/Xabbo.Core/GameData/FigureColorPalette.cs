using System.Collections.Immutable;
using System.Linq;

namespace Xabbo.Core.GameData;

/// <summary>
/// Defines a map of color ID to <see cref="FigurePartColor"/>.
/// </summary>
/// <param name="Id">The ID of the palette.</param>
/// <param name="Colors">The figure part colors.</param>
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
