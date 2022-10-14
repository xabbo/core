using System;
using System.Collections;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public class UserMarketplaceOffers : IUserMarketplaceOffers, ICollection<MarketplaceOffer>
{
    private readonly List<MarketplaceOffer> _list = new();

    public int CreditsWaiting { get; set; }

    public UserMarketplaceOffers() { }

    protected UserMarketplaceOffers(IReadOnlyPacket packet)
    {
        CreditsWaiting = packet.ReadInt();
        _list.AddRange(MarketplaceOffer.ParseMany(packet, false));
    }

    public int Count => _list.Count;
    public bool IsReadOnly => false;

    public void Add(MarketplaceOffer item) => _list.Add(item);
    public bool Contains(MarketplaceOffer item) => _list.Contains(item);
    public bool Remove(MarketplaceOffer item) => _list.Remove(item);
    public void Clear() => _list.Clear();

    public void CopyTo(MarketplaceOffer[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    public IEnumerator<MarketplaceOffer> GetEnumerator() => _list.GetEnumerator();
    IEnumerator<IMarketplaceOffer> IEnumerable<IMarketplaceOffer>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static UserMarketplaceOffers Parse(IReadOnlyPacket packet) => new UserMarketplaceOffers(packet);
}
