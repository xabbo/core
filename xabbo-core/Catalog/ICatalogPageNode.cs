using System;
using System.Collections.Generic;

namespace Xabbo.Core
{
    public interface ICatalogPageNode
    {
        bool IsVisible { get; }
        int Icon { get; }
        int Id { get; }
        string Name { get; }
        string Text { get; }
        IReadOnlyList<int> OfferIds { get; }
        IReadOnlyList<ICatalogPageNode> Children { get; }

        ICatalogPageNode Find(Predicate<ICatalogPageNode> predicate);
        ICatalogPageNode Find(string text);
        ICatalogPageNode Find(int? id = null, string name = null, string text = null);
    }
}
