using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xabbo.Core
{
    public interface IInventoryItem : IItem
    {
        int ItemId { get; }
        FurniCategory Category { get; }
        bool IsGroupable { get; }
    }
}
