namespace Xabbo.Core.GameData;

/// <summary>
/// Defines a figure part color.
/// </summary>
/// <param name="Id">The ID of the color.</param>
/// <param name="Index">The index of the color.</param>
/// <param name="Value">The hexadecimal value of the color.</param>
/// <param name="IsSelectable">Whether the color is selectable in client.</param>
/// <param name="RequiredClubLevel">Whether the color requires a club subscription.</param>
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
    )
    { }
}