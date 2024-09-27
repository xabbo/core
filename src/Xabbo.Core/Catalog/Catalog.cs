using System;
using System.Collections;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="ICatalog"/>
public class Catalog : ICatalog, IEnumerable<CatalogPageNode>, IParserComposer<Catalog>
{
    public CatalogPageNode RootNode { get; set; }
    ICatalogPageNode ICatalog.RootNode => RootNode;
    public bool NewAdditionsAvailable { get; set; }
    public string Type { get; set; }

    public Catalog()
    {
        RootNode = new CatalogPageNode();
        Type = "";
    }

    protected Catalog(in PacketReader p)
    {
        RootNode = p.Parse<CatalogPageNode>();
        NewAdditionsAvailable = p.ReadBool();
        Type = p.ReadString();
    }

    public CatalogPageNode? FindNode(Func<CatalogPageNode, bool> predicate) => RootNode.FindNode(predicate);
    ICatalogPageNode? ICatalog.FindNode(Func<ICatalogPageNode, bool> predicate) => FindNode(predicate);
    public CatalogPageNode? FindNode(string? title = null, string? name = null, int? id = null) => RootNode.FindNode(title, name, id);
    ICatalogPageNode? ICatalog.FindNode(string? title, string? name, int? id) => FindNode(title, name, id);

    public IEnumerator<CatalogPageNode> GetEnumerator() => RootNode.EnumerateDescendantsAndSelf().GetEnumerator();
    IEnumerator<ICatalogPageNode> IEnumerable<ICatalogPageNode>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    void IComposer.Compose(in PacketWriter p)
    {
        p.Compose(RootNode);
        p.WriteBool(NewAdditionsAvailable);
        p.WriteString(Type);
    }

    static Catalog IParser<Catalog>.Parse(in PacketReader p) => new(in p);
}
