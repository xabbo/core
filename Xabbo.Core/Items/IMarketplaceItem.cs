using System;
using Xabbo.Messages;

namespace Xabbo.Core
{
    public interface IMarketplaceItem : IItem, IComposable
    {
        IItemData Data { get; set; }
        int Price { get; set; }
        int Average { get; set; }
        int Offers { get; set; }
    }
}
