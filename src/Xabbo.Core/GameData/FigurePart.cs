namespace Xabbo.Core.GameData;

/// <summary>
/// Defines information about a figure part.
/// </summary>
/// <param name="Type">The type of the figure part.</param>
/// <param name="Id">The ID of the figure part.</param>
/// <param name="IsColorable">Whether the part is colorable.</param>
/// <param name="Index"></param>
/// <param name="ColorIndex"></param>
public sealed record FigurePart(
    FigurePartType Type,
    int Id,
    bool IsColorable = false,
    int Index = 0,
    int ColorIndex = 0
)
{
    internal FigurePart(Xml.FigureData.Part proxy) : this(
        Id: proxy.Id,
        Type: H.GetFigurePartType(proxy.Type),
        IsColorable: proxy.IsColorable,
        Index: proxy.Index,
        ColorIndex: proxy.ColorIndex
    )
    { }
}