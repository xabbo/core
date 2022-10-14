using System;

namespace Xabbo.Core;

public interface ITradeItem : IInventoryItem
{
    int CreationDay { get; }
    int CreationMonth { get; }
    int CreationYear { get; }
}
