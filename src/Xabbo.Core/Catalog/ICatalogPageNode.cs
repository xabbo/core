using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public interface ICatalogPageNode
{
    ICatalog? Catalog { get; }
    ICatalogPageNode? Parent { get; }

    bool IsVisible { get; }
    int Icon { get; }
    int Id { get; }
    string Name { get; }
    string Title { get; }
    IReadOnlyList<int> OfferIds { get; }
    IReadOnlyList<ICatalogPageNode> Children { get; }

    ICatalogPageNode? FindNode(Func<ICatalogPageNode, bool> predicate);
    ICatalogPageNode? FindNode(string? title = null, string? name = null, int? id = null);

    IEnumerable<ICatalogPageNode> EnumerateDescendantsAndSelf();
}
