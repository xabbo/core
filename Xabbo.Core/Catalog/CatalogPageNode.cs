using System;
using System.Collections.Generic;
using System.Linq;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class CatalogPageNode : ICatalogPageNode
    {
        public static CatalogPageNode Parse(IReadOnlyPacket packet) => new CatalogPageNode(packet);

        public bool IsVisible { get; set; }
        public int Icon { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public List<int> OfferIds { get; set; } = new List<int>();
        IReadOnlyList<int> ICatalogPageNode.OfferIds => OfferIds;
        public List<CatalogPageNode> Children { get; set; } = new List<CatalogPageNode>();
        IReadOnlyList<ICatalogPageNode> ICatalogPageNode.Children => Children;

        public CatalogPageNode()
        {
            Name =
            Text = string.Empty;

            OfferIds = new List<int>();
            Children = new List<CatalogPageNode>();
        }

        protected CatalogPageNode(IReadOnlyPacket packet)
        {
            IsVisible = packet.ReadBool();
            Icon = packet.ReadInt();
            Id = packet.ReadInt();
            Name = packet.ReadString();
            Text = packet.ReadString();

            short n = packet.ReadShort();
            for (int i = 0; i < n; i++)
                OfferIds.Add(packet.ReadInt());

            n = packet.ReadShort();
            for (int i = 0; i < n; i++)
                Children.Add(Parse(packet));
        }

        public IEnumerable<CatalogPageNode> EnumerateDescendants()
        {
            var queue = new Queue<CatalogPageNode>(new[] { this });

            CatalogPageNode current;
            while (queue.Any())
            {
                yield return current = queue.Dequeue();
                foreach (var child in current.Children)
                    queue.Enqueue(child);
            }
        }
        IEnumerable<ICatalogPageNode> ICatalogPageNode.EnumerateDescendants() => EnumerateDescendants();

        public CatalogPageNode Find(Predicate<CatalogPageNode> predicate) => EnumerateDescendants().First(x => predicate(x));
        ICatalogPageNode ICatalogPageNode.Find(Predicate<ICatalogPageNode> predicate) => Find(predicate);

        public CatalogPageNode Find(string text) =>
            Find(node => string.Equals(node.Text, text, StringComparison.OrdinalIgnoreCase));
        ICatalogPageNode ICatalogPageNode.Find(string text) => Find(text);

        public CatalogPageNode Find(int? id = null, string name = null, string text = null)
        {
            return Find(node =>
                (id == null || node.Id == id) &&
                (name == null || string.Equals(node.Name, name, StringComparison.OrdinalIgnoreCase)) &&
                (text == null || string.Equals(node.Text, text, StringComparison.OrdinalIgnoreCase))
            );
        }
        ICatalogPageNode ICatalogPageNode.Find(int? id, string name, string text) => Find(id, name, text);
    }
}
