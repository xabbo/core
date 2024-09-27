using System.Collections;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IUserMarketplaceOffers"/>
public sealed class UserMarketplaceOffers : IUserMarketplaceOffers, ICollection<MarketplaceOffer>, IParserComposer<UserMarketplaceOffers>
{
    private readonly List<MarketplaceOffer> _list = [];

    public int CreditsWaiting { get; set; }

    public UserMarketplaceOffers() { }

    private UserMarketplaceOffers(in PacketReader p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        CreditsWaiting = p.ReadInt();
        _list.AddRange(MarketplaceOffer.ParseAll(in p, false));
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.WriteInt(CreditsWaiting);
        p.ComposeArray(_list);
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

    static UserMarketplaceOffers IParser<UserMarketplaceOffers>.Parse(in PacketReader p) => new(in p);
}
