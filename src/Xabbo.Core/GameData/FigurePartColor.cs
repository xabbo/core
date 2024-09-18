namespace Xabbo.Core.GameData;

public sealed class FigurePartColor
{
    public int Id { get; init; }
    public int Index { get; init; }
    public int RequiredClubLevel { get; init; }
    public bool IsSelectable { get; init; }
    public string Value { get; init; } = "";

    public bool IsClubRequired => RequiredClubLevel > 0;

    public FigurePartColor() { }

    internal FigurePartColor(Xml.FigureData.Color proxy)
    {
        Id = proxy.Id;
        Index = proxy.Index;
        RequiredClubLevel = proxy.RequiredClubLevel;
        IsSelectable = proxy.IsSelectable;
        Value = proxy.Value;
    }
}