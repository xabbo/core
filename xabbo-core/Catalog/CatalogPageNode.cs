using System;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class CatalogPageNode : ICatalogPageNode
    {
        private static IEnumerable<CatalogPageNode> EnumerateTree(CatalogPageNode node)
        {
            yield return node;
            foreach (var child in node.Children)
                foreach (var grandChild in EnumerateTree(child))
                    yield return grandChild;
        }

        public static CatalogPageNode Parse(Packet packet) => new CatalogPageNode(packet);

        public bool IsVisible { get; set; }
        public int Icon { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public List<int> OfferIds { get; set; } = new List<int>();
        IReadOnlyList<int> ICatalogPageNode.OfferIds => OfferIds;
        public List<CatalogPageNode> Children { get; set; } = new List<CatalogPageNode>();
        IReadOnlyList<ICatalogPageNode> ICatalogPageNode.Children => Children;

        public CatalogPageNode() { }

        internal CatalogPageNode(Packet packet)
        {
            IsVisible = packet.ReadBool();
            Icon = packet.ReadInt();
            Id = packet.ReadInt();
            Name = packet.ReadString();
            Text = packet.ReadString();

            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
                OfferIds.Add(packet.ReadInt());

            n = packet.ReadInt();
            for (int i = 0; i < n; i++)
                Children.Add(Parse(packet));
        }

        public CatalogPageNode Find(Predicate<CatalogPageNode> predicate)
        {
            foreach (var node in EnumerateTree(this))
                if (predicate(node))
                    return node;
            return null;
        }

        ICatalogPageNode ICatalogPageNode.Find(Predicate<ICatalogPageNode> predicate) => Find(predicate);

        public CatalogPageNode Find(string text) =>
            Find(node => node.Text.Equals(text, StringComparison.InvariantCultureIgnoreCase));

        ICatalogPageNode ICatalogPageNode.Find(string text) => Find(text);

        public CatalogPageNode Find(int? id = null, string name = null, string text = null)
        {
            return Find(node =>
                (!id.HasValue || node.Id == id) &&
                (name == null || node.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) &&
                (text == null || node.Text.Equals(text, StringComparison.InvariantCultureIgnoreCase))
            );
        }

        ICatalogPageNode ICatalogPageNode.Find(int? id, string name, string text) => Find(id, name, text);
    }
}
