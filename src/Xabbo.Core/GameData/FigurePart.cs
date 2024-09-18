namespace Xabbo.Core.GameData;

public sealed class FigurePart
{
    public int Id { get; init; }
    public FigurePartType Type { get; init; }
    public bool IsColorable { get; init; }
    public int Index { get; init; }
    public int ColorIndex { get; init; }

    public FigurePart() { }

    internal FigurePart(Xml.FigureData.Part proxy)
    {
        Id = proxy.Id;
        Type = H.GetFigurePartType(proxy.Type);
        IsColorable = proxy.IsColorable;
        Index = proxy.Index;
        ColorIndex = proxy.ColorIndex;
    }
}