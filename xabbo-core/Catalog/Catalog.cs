using System;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class Catalog
    {
        public static Catalog Parse(Packet packet) => new Catalog(packet);

        public CatalogPageNode Root { get; set; }
        public bool UnknownBoolA { get; set; }
        public string Mode { get; set; }

        public Catalog() { }

        internal Catalog(Packet packet)
        {
            Root = CatalogPageNode.Parse(packet);
            UnknownBoolA = packet.ReadBoolean();
            Mode = packet.ReadString();
        }

        public CatalogPageNode FindPage(string pageName) => Root.FindPage(pageName);
        public CatalogPageNode FindPage(int? id = null, string name = null, string localization = null) => Root.FindPage(id, name, localization);
        public CatalogPageNode FindPage(Predicate<CatalogPageNode> predicate) => Root.FindPage(predicate);
    }
}
