namespace Xabbo.Core.GameData;

public sealed record FigurePartColor(
    int Id,
    int Index,
    string Value,
    bool IsSelectable = false,
    int RequiredClubLevel = 0
)
{
    public bool IsClubRequired => RequiredClubLevel > 0;

    internal FigurePartColor(Xml.FigureData.Color proxy) : this(
        Id: proxy.Id,
        Index: proxy.Index,
        Value: proxy.Value,
        IsSelectable: proxy.IsSelectable,
        RequiredClubLevel: proxy.RequiredClubLevel
    ) { }
}