using System;
using System.Collections.Generic;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class CatalogPage : ICatalogPage
    {
        public static CatalogPage Parse(IReadOnlyPacket packet) => new CatalogPage(packet);

        public int Id { get; set; }
        public string Mode { get; set; }
        public string LayoutCode { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        IReadOnlyList<string> ICatalogPage.Images => Images;
        public List<string> Texts { get; set; } = new List<string>();
        IReadOnlyList<string> ICatalogPage.Texts => Texts;
        public List<CatalogOffer> Offers { get; set; } = new List<CatalogOffer>();
        IReadOnlyList<ICatalogOffer> ICatalogPage.Offers => Offers;
        public int UnknownIntA { get; set; }
        public bool AcceptSeasonCurrencyAsCredits { get; set; }
        public List<CatalogPageData> Data { get; set; } = new List<CatalogPageData>();
        IReadOnlyList<ICatalogPageData> ICatalogPage.Data => Data;

        public CatalogPage()
        {
            Mode =
            LayoutCode = string.Empty;

            Images = new List<string>();
            Texts = new List<string>();
            Offers = new List<CatalogOffer>();
            Data = new List<CatalogPageData>();
        }

        protected CatalogPage(IReadOnlyPacket packet)
        {
            Id = packet.ReadInt();
            Mode = packet.ReadString();
            LayoutCode = packet.ReadString();

            int n = packet.ReadShort();
            for (int i = 0; i < n; i++)
                Images.Add(packet.ReadString());

            n = packet.ReadShort();
            for (int i = 0; i < n; i++)
                Texts.Add(packet.ReadString());

            n = packet.ReadShort();
            for (int i = 0; i < n; i++)
                Offers.Add(CatalogOffer.Parse(packet));

            UnknownIntA = packet.ReadInt();
            AcceptSeasonCurrencyAsCredits = packet.ReadBool();
            if (packet.Available > 0)
            {
                n = packet.ReadInt();
                for (int i = 0; i < n; i++)
                    Data.Add(CatalogPageData.Parse(packet));
            }
        }
    }
}