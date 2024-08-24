using System.Collections;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class UserMarketplaceOffers : IUserMarketplaceOffers, ICollection<MarketplaceOffer>, IComposer, IParser<UserMarketplaceOffers>
{
    private readonly List<MarketplaceOffer> _list = [];

    public int CreditsWaiting { get; set; }

    public UserMarketplaceOffers() { }

    private UserMarketplaceOffers(in PacketReader p)
    {
        CreditsWaiting = p.Read<int>();
        _list.AddRange(MarketplaceOffer.ParseAll(in p, false));
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

    public static UserMarketplaceOffers Parse(in PacketReader packet) => new(in packet);

    public void Compose(in PacketWriter p)
    {
        p.Write(CreditsWaiting);
        p.Write(_list);
    }
}
