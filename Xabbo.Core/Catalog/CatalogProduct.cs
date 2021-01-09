using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class CatalogProduct : ICatalogProduct
    {
        public static CatalogProduct Parse(IReadOnlyPacket packet) => new CatalogProduct(packet);

        public ItemType Type { get; set; }
        public int Kind { get; set; }
        public string Variant { get; set; }
        public int Count { get; set; }
        public bool IsLimited { get; set; }
        public int LimitedTotal { get; set; }
        public int LimitedRemaining { get; set; }

        public bool IsFloorItem => Type == ItemType.Floor;
        public bool IsWallItem => Type == ItemType.Wall;
        long IItem.Id => -1;

        public CatalogProduct()
        {
            Variant = string.Empty;
        }

        protected CatalogProduct(IReadOnlyPacket packet)
        {
            Type = H.ToItemType(packet.ReadString());
            if (Type == ItemType.Badge)
            {
                Variant = packet.ReadString();
                Count = 0;
            }
            else
            {
                Kind = packet.ReadInt();
                Variant = packet.ReadString();
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
