using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class CatalogPageData
    {
        public static CatalogPageData Parse(Packet packet) => new CatalogPageData(packet);

        public int Position { get; set; }
        public string StringA { get; set; }
        public string StringB { get; set; }
        public int Type { get; set; }
        public string StringC { get; set; }
        public int IntA { get; set; }
        public string StringD { get; set; }
        public int SecondsToExpiration { get; set; }

        public CatalogPageData() { }

        internal CatalogPageData(Packet packet)
        {
            Position = packet.ReadInteger();
            StringA = packet.ReadString();
            StringB = packet.ReadString();
            Type = packet.ReadInteger();
            if (Type == 0)
            {
                StringC = packet.ReadString();
            }
            else if (Type == 1)
            {
                IntA = packet.ReadInteger();
            }
            else if (Type == 2)
            {
                StringD = packet.ReadString();
            }
            SecondsToExpiration = packet.ReadInteger();
        }
    }
}
