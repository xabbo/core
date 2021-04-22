using System;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class Catalog : ICatalog
    {
        public CatalogPageNode Root { get; set; }
        ICatalogPageNode ICatalog.Root => Root;
        public bool UnknownBoolA { get; set; }
        public string Mode { get; set; }

        public Catalog()
        {
            Root = new CatalogPageNode();
            Mode = string.Empty;
        }
        
        protected Catalog(IReadOnlyPacket packet, ClientType clientType)
        {
            Root = CatalogPageNode.Parse(packet, clientType);
            UnknownBoolA = packet.ReadBool();
            Mode = packet.ReadString();
        }

        public CatalogPageNode Find(Predicate<CatalogPageNode> predicate) => Root.Find(predicate);
        ICatalogPageNode ICatalog.Find(Predicate<ICatalogPageNode> predicate) => Find(predicate);
        public CatalogPageNode Find(string name) => Root.Find(name);
        ICatalogPageNode ICatalog.Find(string name) => Find(name);
        public CatalogPageNode Find(int? id = null, string name = null, string text = null) => Root.Find(id, name, text);
        ICatalogPageNode ICatalog.Find(int? id, string name, string text) => Find(id, name, text);

        public static Catalog Parse(IReadOnlyPacket packet, ClientType clientType = ClientType.Unknown)
        {
            return new Catalog(packet, clientType);
        }
    }
}
