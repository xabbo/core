using System;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class CatalogPageData : ICatalogPageData
    {
        public static CatalogPageData Parse(IReadOnlyPacket packet) => new CatalogPageData(packet);

        public int Position { get; set; }
        public string String1 { get; set; }
        public string String2 { get; set; }
        public int Type { get; set; }
        public string String3 { get; set; }
        public int Int3 { get; set; }
        public string String4 { get; set; }
        public int SecondsToExpiration { get; set; }

        public CatalogPageData() { }

        protected CatalogPageData(IReadOnlyPacket packet)
        {
            Position = packet.ReadInt();
            String1 = packet.ReadString();
            String2 = packet.ReadString();
            Type = packet.ReadInt();

            switch (Type)
            {
                case 0:
                    String3 = packet.ReadString();
                    break;
                case 1:
                    Int3 = packet.ReadInt();
                    break;
                case 2:
                    String4 = packet.ReadString();
                    break;
                default:
                    break;
            }

            SecondsToExpiration = packet.ReadInt();
        }
    }
}
