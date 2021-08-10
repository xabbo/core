using System;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class CatalogPageNode : ICatalogPageNode
    {
        public Catalog Catalog { get; set; }
        ICatalog ICatalogPageNode.Catalog => Catalog;
        public CatalogPageNode Parent { get; set; }
        ICatalogPageNode ICatalogPageNode.Parent => Parent;

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

            OfferIds = new List<int>();
            Children = new List<CatalogPageNode>();
        }

        protected internal CatalogPageNode(IReadOnlyPacket packet,
            Catalog? catalog = null, CatalogPageNode? parent = null)
        {
            IsVisible = packet.ReadBool();
            Icon = packet.ReadInt();
            Id = packet.ReadInt();
            Name = packet.ReadString();
            Title = packet.ReadString();

            short n = packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
                OfferIds.Add(packet.ReadInt());

            n = packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
                Children.Add(new CatalogPageNode(packet, catalog, this));
        }

        public void Compose(IPacket packet)
        {
            packet
                .WriteBool(IsVisible)
                .WriteInt(Icon)
                .WriteInt(Id)
                .WriteString(Name)
                .WriteString(Title)
                .WriteValues(OfferIds, Children);
        }

        public IEnumerable<CatalogPageNode> EnumerateDescendantsAndSelf()
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
        IEnumerable<ICatalogPageNode> ICatalogPageNode.EnumerateDescendantsAndSelf() => EnumerateDescendantsAndSelf();

        public CatalogPageNode FindNode(Func<CatalogPageNode, bool> predicate) => EnumerateDescendantsAndSelf().First(x => predicate(x));
        ICatalogPageNode ICatalogPageNode.FindNode(Func<ICatalogPageNode, bool> predicate) => FindNode(predicate);

        public CatalogPageNode FindNode(string? title = null, string? name = null, int? id = null)
        {
            return FindNode(node =>
                (id is null || node.Id == id) &&
                (name is null || string.Equals(node.Name, name, StringComparison.OrdinalIgnoreCase)) &&
                (title is null || string.Equals(node.Title, title, StringComparison.OrdinalIgnoreCase))
            );
        }
        ICatalogPageNode ICatalogPageNode.FindNode(string? title, string? name, int? id) => FindNode(title, name, id);

        public static CatalogPageNode Parse(IReadOnlyPacket packet) => new CatalogPageNode(packet);
    }
}
