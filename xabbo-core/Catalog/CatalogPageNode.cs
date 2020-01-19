using System;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class CatalogPageNode
    {
        public static CatalogPageNode Parse(Packet packet) => new CatalogPageNode(packet);

        public bool IsVisible { get; set; }
        public int Icon { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Localization { get; set; }
        public List<int> OfferIds { get; set; } = new List<int>();
        public List<CatalogPageNode> Children { get; set; } = new List<CatalogPageNode>();

        public CatalogPageNode() { }

        internal CatalogPageNode(Packet packet)
        {
            IsVisible = packet.ReadBoolean();
            Icon = packet.ReadInteger();
            Id = packet.ReadInteger();
            Name = packet.ReadString();
            Localization = packet.ReadString();

            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                OfferIds.Add(packet.ReadInteger());

            n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                Children.Add(Parse(packet));
        }

        public CatalogPageNode FindPage(string pageName)
        {
            if (Name.Equals(pageName, StringComparison.InvariantCultureIgnoreCase)) return this;

            foreach (var childNode in Children)
            {
                var matchingNode = childNode.FindPage(pageName);
                if (matchingNode != null) return matchingNode;
            }

            return null;
        }

        public CatalogPageNode FindPage(int? id = null, string name = null, string localization = null)
        {
            return FindPage(node =>
                (!id.HasValue || Id == id) &&
                (name == null || Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) &&
                (localization == null || Localization.Equals(localization, StringComparison.InvariantCultureIgnoreCase))
            );
        }

        public CatalogPageNode FindPage(Predicate<CatalogPageNode> predicate)
        {
            if (predicate(this)) return this;

            foreach (var childNode in Children)
            {
                var matchingNode = childNode.FindPage(predicate);
                if (matchingNode != null) return matchingNode;
            }

            return null;
        }
    }
}
