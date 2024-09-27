using System;
using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Represents a hierarchical tree structure of catalog page nodes.
/// </summary>
public interface ICatalog : IEnumerable<ICatalogPageNode>
{
    /// <summary>
    /// The root node.
    /// </summary>
    ICatalogPageNode RootNode { get; }

    bool NewAdditionsAvailable { get; }

    /// <summary>
    /// The type of the catalog.
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Searches the catalog for a matching page node.
    /// </summary>
    /// <param name="title">The title of the catalog page.</param>
    /// <param name="name">The name of the catalog page.</param>
    /// <param name="id">The ID of the catalog page.</param>
    /// <returns>The first matching catalog page node, or <c>null</c> if it was not found.</returns>
    ICatalogPageNode? FindNode(string? title = null, string? name = null, int? id = null);

    /// <summary>
    /// Searches the catalog for a matching page node.
    /// </summary>
    /// <param name="predicate">A function to test each node for a condition.</param>
    /// <returns>The first matching catalog page node, or <c>null</c> if it was not found.</returns>
    ICatalogPageNode? FindNode(Func<ICatalogPageNode, bool> predicate);
}
