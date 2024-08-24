using System.Collections.Generic;

namespace Xabbo.Core.Events;

public sealed class WallItemsEventArgs(IEnumerable<IWallItem> items)
{
    public IReadOnlyCollection<IWallItem> Items { get; } = new List<IWallItem>(items).AsReadOnly();
}
