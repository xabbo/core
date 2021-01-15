using System;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    public class Catalog : ICatalog
    {
        public static Catalog Parse(IReadOnlyPacket packet) => new Catalog(packet);

        public CatalogPageNode Root { get; set; }
        ICatalogPageNode ICatalog.Root => Root;
        public bool UnknownBoolA { get; set; }
        public string Mode { get; set; }

        public Catalog()
        {
            Root = new CatalogPageNode();
            Mode = string.Empty;
        }
        
        protected Catalog(IReadOnlyPacket packet)
        {
            Root = CatalogPageNode.Parse(packet);
            UnknownBoolA = packet.ReadBool();
            Mode = packet.ReadString();
        }

        public CatalogPageNode Find(Predicate<CatalogPageNode> predicate) => Root.Find(predicate);
        ICatalogPageNode ICatalog.Find(Predicate<ICatalogPageNode> predicate) => Find(predicate);
        public CatalogPageNode Find(string name) => Root.Find(name);
        ICatalogPageNode ICatalog.Find(string name) => Find(name);
        public CatalogPageNode Find(int? id = null, string name = null, string text = null) => Root.Find(id, name, text);
        ICatalogPageNode ICatalog.Find(int? id, string name, string text) => Find(id, name, text);
    }
}
