using System;
using System.Collections.Generic;

namespace Xabbo.Core
{
    public interface ITradeOffer
    {
        int UserId { get; }
        IReadOnlyList<ITradeItem> Items { get; }
        int FurniCount { get; }
        int CreditCount { get; }
    }
}
