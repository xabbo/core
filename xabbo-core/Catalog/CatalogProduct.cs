using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class CatalogProduct : ICatalogProduct
    {
        public static CatalogProduct Parse(Packet packet) => new CatalogProduct(packet);

        public string Type { get; set; }
        public int Kind { get; set; }
        public string Variation { get; set; }
        public int Count { get; set; }
        public bool IsLimited { get; set; }
        public int LimitedTotal { get; set; }
        public int LimitedRemaining { get; set; }

        public CatalogProduct() { }

        internal CatalogProduct(Packet packet)
        {
            Type = packet.ReadString();
            if (Type.ToLower().Equals("b"))
            {
                Variation = packet.ReadString();
                Count = 0;
            }
            else
            {
                Kind = packet.ReadInt();
                Variation = packet.ReadString();
                Count = packet.ReadInt();
                IsLimited = packet.ReadBool();
                if (IsLimited)
                {
                    LimitedTotal = packet.ReadInt();
                    LimitedRemaining = packet.ReadInt();
                }
            }
        }
    }
}
