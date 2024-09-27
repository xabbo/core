using System;
using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Represents a page node in the catalog.
/// Defines basic information about a <see cref="ICatalogPage"/>.
/// </summary>
public interface ICatalogPageNode
{
    /// <summary>
    /// The catalog this page node belongs to.
    /// </summary>
    ICatalog? Catalog { get; }

    /// <summary>
    /// The parent of this node. May be null if this is the root node.
    /// </summary>
    ICatalogPageNode? Parent { get; }

    /// <summary>
    /// Whether the page is visible in the catalog.
    /// </summary>
    bool IsVisible { get; }

    /// <summary>
    /// The ID of the icon shown in the catalog.
    /// </summary>
    int Icon { get; }

    /// <summary>
    /// The ID of the catalog page.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// The string identifier of the catalog page.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The localized title of the catalog page.
    /// </summary>
    string Title { get; }

    IReadOnlyList<int> OfferIds { get; }

    /// <summary>
    /// The list of child nodes.
    /// </summary>
    IReadOnlyList<ICatalogPageNode> Children { get; }

    /// <summary>
    /// Searches this node and its descendants for a matching page node.
    /// </summary>
    /// <param name="predicate">A function to test each node for a condition.</param>
    /// <returns>The first matching catalog page node, or <c>null</c> if it was not found.</returns>
    ICatalogPageNode? FindNode(Func<ICatalogPageNode, bool> predicate);

    /// <summary>
    /// Searches this node and its descendants for a matching page node.
    /// </summary>
    /// <param name="title">The title of the catalog page.</param>
    /// <param name="name">The name of the catalog page.</param>
    /// <param name="id">The ID of the catalog page.</param>
    /// <returns>The first matching catalog page node, or <c>null</c> if it was not found.</returns>
    ICatalogPageNode? FindNode(string? title = null, string? name = null, int? id = null);

    /// <summary>
    /// Enumerates this node and its descendants.
    /// </summary>
    IEnumerable<ICatalogPageNode> EnumerateDescendantsAndSelf();
}
