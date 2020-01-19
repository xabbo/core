using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class CatalogProduct
    {
        public static CatalogProduct Parse(Packet packet) => new CatalogProduct(packet);

        public string ItemType { get; set; }
        public int FurniId { get; set; }
        public string StringB { get; set; }
        public int Count { get; set; }
        public bool IsLimited { get; set; }
        public int UniqueLimitedItemSeriesSize { get; set; }
        public int UniqueLimitedItemsLeft { get; set; }

        public CatalogProduct() { }

        internal CatalogProduct(Packet packet)
        {
            ItemType = packet.ReadString();
            if (ItemType.ToLower().Equals("b"))
            {
                StringB = packet.ReadString();
                Count = 0;
            }
            else
            {
                FurniId = packet.ReadInteger();
                StringB = packet.ReadString();
                Count = packet.ReadInteger();
                IsLimited = packet.ReadBoolean();
                if (IsLimited)
                {
                    UniqueLimitedItemSeriesSize = packet.ReadInteger();
                    UniqueLimitedItemsLeft = packet.ReadInteger();
                }
            }
        }
    }
}
