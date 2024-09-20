namespace Xabbo.Core.GameData;

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