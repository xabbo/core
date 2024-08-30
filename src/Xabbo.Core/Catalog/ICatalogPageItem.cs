namespace Xabbo.Core;

public interface ICatalogPageItem
{
    int Position { get; }
    int Type { get; }
    int SecondsToExpiration { get; set; }
}
