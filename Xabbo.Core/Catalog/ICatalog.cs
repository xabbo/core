using System;

namespace Xabbo.Core
{
    public interface ICatalog
    {
        ICatalogPageNode Root { get; }
        string Mode { get; }

        ICatalogPageNode Find(Predicate<ICatalogPageNode> predicate);
        ICatalogPageNode Find(string text);
        ICatalogPageNode Find(int? id = null, string name = null, string text = null);
    }
}
