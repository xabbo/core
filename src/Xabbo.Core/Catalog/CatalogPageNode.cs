using System;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="ICatalogPageNode"/>
public class CatalogPageNode : ICatalogPageNode, IParserComposer<CatalogPageNode>
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
    public List<int> OfferIds { get; set; }
    IReadOnlyList<int> ICatalogPageNode.OfferIds => OfferIds;
    public List<CatalogPageNode> Children { get; set; }
    IReadOnlyList<ICatalogPageNode> ICatalogPageNode.Children => Children;

    public CatalogPageNode()
    {
        Name =
        Title = "";

        OfferIds = [];
        Children = [];
    }

    protected internal CatalogPageNode(in PacketReader p,
        Catalog? catalog = null, CatalogPageNode? parent = null)
    {
        Parent = parent;

        IsVisible = p.ReadBool();
        Icon = p.ReadInt();
        Id = p.ReadInt();
        Name = p.ReadString();
        Title = p.ReadString();

        int n = p.ReadLength();
        OfferIds = new List<int>(n);
        for (int i = 0; i < n; i++)
            OfferIds.Add(p.ReadInt());

        n = p.ReadLength();
        Children = new List<CatalogPageNode>(n);
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

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteBool(IsVisible);
        p.WriteInt(Icon);
        p.WriteInt(Id);
        p.WriteString(Name);
        p.WriteString(Title);
        p.WriteIntArray(OfferIds);
        p.ComposeArray(Children);
    }

    static CatalogPageNode IParser<CatalogPageNode>.Parse(in PacketReader p) => new(in p);
}
