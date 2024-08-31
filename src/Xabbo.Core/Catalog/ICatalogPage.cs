using System.Collections.Generic;

namespace Xabbo.Core;

public interface ICatalogPage
{
    int Id { get; }
    string CatalogType { get; }
    string LayoutCode { get; }
    IReadOnlyList<string> Images { get; }
    IReadOnlyList<string> Texts { get; }
    IReadOnlyList<ICatalogOffer> Offers { get; }
    bool AcceptSeasonCurrencyAsCredits { get; }
    IReadOnlyList<ICatalogPageItem> Data { get; }
}
