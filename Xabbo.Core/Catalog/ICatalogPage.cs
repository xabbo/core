using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public interface ICatalogPage : IComposable
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
}
