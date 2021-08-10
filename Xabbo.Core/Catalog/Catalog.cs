using System;
using System.Collections;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class Catalog : ICatalog, IEnumerable<CatalogPageNode>
    {
        public CatalogPageNode RootNode { get; set; }
        ICatalogPageNode ICatalog.RootNode => RootNode;
        public bool NewAdditionsAvailable { get; set; }
        public string Type { get; set; }

        public Catalog()
        {
            RootNode = new CatalogPageNode();
            Type = string.Empty;
        }
        
        protected Catalog(IReadOnlyPacket packet)
        {
            RootNode = CatalogPageNode.Parse(packet);
            NewAdditionsAvailable = packet.ReadBool();
            Type = packet.ReadString();
        }

        public void Compose(IPacket packet)
        {
            packet
                .Write(RootNode)
                .WriteBool(NewAdditionsAvailable)
                .WriteString(Type);
        }

        public CatalogPageNode FindNode(Func<CatalogPageNode, bool> predicate) => RootNode.FindNode(predicate);
        ICatalogPageNode? ICatalog.FindNode(Func<ICatalogPageNode, bool> predicate) => FindNode(predicate);
        public CatalogPageNode FindNode(string? title, string? name, int? id) => RootNode.FindNode(title, name, id);
        ICatalogPageNode? ICatalog.FindNode(string? title, string? name, int? id) => FindNode(title, name, id);

        public IEnumerator<CatalogPageNode> GetEnumerator() => RootNode.EnumerateDescendantsAndSelf().GetEnumerator();
        IEnumerator<ICatalogPageNode> IEnumerable<ICatalogPageNode>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static Catalog Parse(IReadOnlyPacket packet) => new Catalog(packet);
    }
}
