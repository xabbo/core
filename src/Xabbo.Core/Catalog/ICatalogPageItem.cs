namespace Xabbo.Core;

/// <summary>
/// Represents an item on a catalog page.
/// </summary>
public interface ICatalogPageItem
{
    int Position { get; }
    int Type { get; }
    int SecondsToExpiration { get; set; }
}
