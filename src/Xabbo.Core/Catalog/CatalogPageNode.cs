using System;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;

namespace Xabbo.Core;

public class CatalogPageNode : ICatalogPageNode, IParser<CatalogPageNode>, IComposer
{
    public Catalog? Catalog { get; set; }
    ICatalog? ICatalogPageNode.Catalog => Catalog;
    public CatalogPageNode? Parent { get; set; }
    ICatalogPageNode? ICatalogPageNode.Parent => Parent;

    public bool IsVisible { get; set; }
    public int Icon { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public List<int> OfferIds { get; set; } = new List<int>();
    IReadOnlyList<int> ICatalogPageNode.OfferIds => OfferIds;
    public List<CatalogPageNode> Children { get; set; } = new List<CatalogPageNode>();
    IReadOnlyList<ICatalogPageNode> ICatalogPageNode.Children => Children;

    public CatalogPageNode()
    {
        Name =
        Title = string.Empty;

        OfferIds = [];
        Children = [];
    }

    protected internal CatalogPageNode(in PacketReader p,
        Catalog? catalog = null, CatalogPageNode? parent = null)
    {
        Parent = parent;

        IsVisible = p.Read<bool>();
        Icon = p.Read<int>();
        Id = p.Read<int>();
        Name = p.Read<string>();
        Title = p.Read<string>();

        int n = p.Read<Length>();
        for (int i = 0; i < n; i++)
            OfferIds.Add(p.Read<int>());

        n = p.Read<Length>();
        for (int i = 0; i < n; i++)
            Children.Add(new CatalogPageNode(in p, catalog, this));
    }


    public IEnumerable<CatalogPageNode> EnumerateDescendantsAndSelf()
    {
        var queue = new Queue<CatalogPageNode>([this]);

        CatalogPageNode current;
        while (queue.Count != 0)
        {
            yield return current = queue.Dequeue();
            foreach (var child in current.Children)
                queue.Enqueue(child);
        }
    }
    IEnumerable<ICatalogPageNode> ICatalogPageNode.EnumerateDescendantsAndSelf() => EnumerateDescendantsAndSelf();

    public CatalogPageNode? FindNode(Func<CatalogPageNode, bool> predicate) => EnumerateDescendantsAndSelf().FirstOrDefault(x => predicate(x));
    ICatalogPageNode? ICatalogPageNode.FindNode(Func<ICatalogPageNode, bool> predicate) => FindNode(predicate);

    public CatalogPageNode? FindNode(string? title = null, string? name = null, int? id = null)
    {
        return FindNode(node =>
            (id is null || node.Id == id) &&
            (name is null || string.Equals(node.Name, name, StringComparison.OrdinalIgnoreCase)) &&
            (title is null || string.Equals(node.Title, title, StringComparison.OrdinalIgnoreCase))
        );
    }
    ICatalogPageNode? ICatalogPageNode.FindNode(string? title, string? name, int? id) => FindNode(title, name, id);

    public void Compose(in PacketWriter p)
    {
        p.Write(IsVisible);
        p.Write(Icon);
        p.Write(Id);
        p.Write(Name);
        p.Write(Title);
        p.Write(OfferIds);
        p.Write(Children);
    }

    public static CatalogPageNode Parse(in PacketReader p) => new(in p);
}
