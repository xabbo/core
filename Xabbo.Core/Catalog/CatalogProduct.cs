using System;

using Xabbo.Messages;

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
            Type = packet.Protocol switch
            {
                ClientType.Flash => H.ToItemType(packet.ReadString()),
                ClientType.Unity => H.ToItemType(packet.ReadShort()),
                _ => throw new Exception($"Unknown client protocol: {packet.Protocol}.")
            };

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
                // int ?
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
