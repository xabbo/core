using System;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class CatalogPageData : ICatalogPageData
    {
        public int Position { get; set; }
        public string String1 { get; set; }
        public string String2 { get; set; }
        public int Type { get; set; }
        public string String3 { get; set; }
        public int Int3 { get; set; }
        public string String4 { get; set; }
        public int SecondsToExpiration { get; set; }

        public CatalogPageData()
        {
            String1 =
            String2 =
            String3 =
            String4 = string.Empty;
        }

        protected CatalogPageData(IReadOnlyPacket packet, ClientType clientType)
            : this()
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

        public static CatalogPageData Parse(IReadOnlyPacket packet, ClientType clientType = ClientType.Unknown)
        {
            return new CatalogPageData(packet, clientType);
        }
    }
}
