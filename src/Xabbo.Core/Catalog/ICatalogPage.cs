using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Represents a page in the catalog.
/// </summary>
public interface ICatalogPage
{
    /// <summary>
    /// The ID of the catalog page.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// The type of catalog the page is for.
    /// </summary>
    string CatalogType { get; }

    string LayoutCode { get; }

    IReadOnlyList<string> Images { get; }

    IReadOnlyList<string> Texts { get; }

    /// <summary>
    /// The list of offers on the page.
    /// </summary>
    IReadOnlyList<ICatalogOffer> Offers { get; }

    bool AcceptSeasonCurrencyAsCredits { get; }

    IReadOnlyList<ICatalogPageItem> Data { get; }
}
