using System;
using System.Collections.Generic;

namespace Xabbo.Core
{
    public interface ICatalogPage
    {
        int Id { get; }
        string Mode { get; }
        string LayoutCode { get; }
        IReadOnlyList<string> Images { get; }
        IReadOnlyList<string> Texts { get; }
        IReadOnlyList<ICatalogOffer> Offers { get; }
        bool AcceptSeasonCurrencyAsCredits { get; }
        IReadOnlyList<ICatalogPageData> Data { get; }
    }
}
