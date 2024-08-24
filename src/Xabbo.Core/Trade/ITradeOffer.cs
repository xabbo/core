using System.Collections.Generic;

namespace Xabbo.Core;

public interface ITradeOffer
{
    Id UserId { get; }
    IReadOnlyList<ITradeItem> Items { get; }
    int FurniCount { get; }
    int CreditCount { get; }
}
