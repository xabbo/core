using System.Linq;
using System.Collections.Generic;

namespace Xabbo.Core.Events;

public sealed class FloorItemsEventArgs(IEnumerable<IFloorItem> items)
{
    public IFloorItem[] Items { get; } = items.ToArray();
}
