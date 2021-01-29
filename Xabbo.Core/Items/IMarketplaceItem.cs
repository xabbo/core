using System;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public interface IMarketplaceItem : IItem, IPacketData
    {
        IItemData Data { get; set; }
        int Price { get; set; }
        int Average { get; set; }
        int Offers { get; set; }
    }
}
