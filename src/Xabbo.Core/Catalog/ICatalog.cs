using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public interface ICatalog : IEnumerable<ICatalogPageNode>, IComposable
{
    /// <summary>
    /// Gets the root <see cref="ICatalogPageNode"/>.
    /// </summary>
    ICatalogPageNode RootNode { get; }
    bool NewAdditionsAvailable { get; }
    /// <summary>
    /// Gets the type of the catalog.
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Searches the catalog for a matching <see cref="ICatalogPageNode"/>.
    /// </summary>
    /// <param name="title">The title of the catalog page.</param>
    /// <param name="name">The name of the catalog page.</param>
    /// <param name="id">The ID of the catalog page.</param>
    /// <returns>The first matching catalog page, or <c>null</c> if it was not found.</returns>
    ICatalogPageNode? FindNode(string? title = null, string? name = null, int? id = null);

    /// <summary>
    /// Searches the catalog for a matching <see cref="ICatalogPageNode"/>.
    /// </summary>
    /// <param name="predicate">A function to test each node for a condition.</param>
    /// <returns>The first matching catalog page, or <c>null</c> if it was not found.</returns>
    ICatalogPageNode? FindNode(Func<ICatalogPageNode, bool> predicate);
}
