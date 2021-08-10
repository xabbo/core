using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class CatalogPage : ICatalogPage
    {
        public int Id { get; set; }
        public string CatalogType { get; set; }
        public string LayoutCode { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        IReadOnlyList<string> ICatalogPage.Images => Images;
        public List<string> Texts { get; set; } = new List<string>();
        IReadOnlyList<string> ICatalogPage.Texts => Texts;
        public List<CatalogOffer> Offers { get; set; } = new List<CatalogOffer>();
        IReadOnlyList<ICatalogOffer> ICatalogPage.Offers => Offers;
        public int OfferId { get; set; }
        public bool AcceptSeasonCurrencyAsCredits { get; set; }
        public List<CatalogPageItem> FrontPageItems { get; set; } = new List<CatalogPageItem>();
        IReadOnlyList<ICatalogPageItem> ICatalogPage.Data => FrontPageItems;

        public CatalogPage()
        {
            CatalogType =
            LayoutCode = string.Empty;

            Images = new List<string>();
            Texts = new List<string>();
            Offers = new List<CatalogOffer>();
            FrontPageItems = new List<CatalogPageItem>();
        }

        protected CatalogPage(IReadOnlyPacket packet)
        {
            Id = packet.ReadInt();
            CatalogType = packet.ReadString();
            LayoutCode = packet.ReadString();

            short n = packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
                Images.Add(packet.ReadString());

            n = packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
                Texts.Add(packet.ReadString());

            n = packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
            {
                CatalogOffer offer = CatalogOffer.Parse(packet);
                offer.Page = this;
                Offers.Add(offer);
            }

            OfferId = packet.ReadInt();
            AcceptSeasonCurrencyAsCredits = packet.ReadBool();
            if (packet.Available > 0)
            {
                n = packet.ReadLegacyShort();
                for (int i = 0; i < n; i++)
                    FrontPageItems.Add(CatalogPageItem.Parse(packet));
            }
        }

        public void Compose(IPacket packet)
        {
            packet
                .WriteInt(Id)
                .WriteString(CatalogType)
                .WriteString(LayoutCode)
                .WriteValues(Images, Texts, Offers)
                .WriteInt(OfferId)
                .WriteBool(AcceptSeasonCurrencyAsCredits)
                .WriteValues(FrontPageItems);
        }

        public static CatalogPage Parse(IReadOnlyPacket packet) => new CatalogPage(packet);
    }
}