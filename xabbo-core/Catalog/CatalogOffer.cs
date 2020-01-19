using System;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class CatalogOffer
    {
        public static CatalogOffer Parse(Packet packet) => new CatalogOffer(packet);

        public int Id { get; set; }
        public string FurniLine { get; set; }
        public bool IsRentable { get; set; } // @Sulakore
        public int PriceInCredits { get; set; }
        public int PriceInActivityPoints { get; set; }
        public ActivityPointType ActivityPointType { get; set; }
        public bool CanPurchaseAsGift { get; set; } // @Sulakore
        public List<CatalogProduct> Products { get; set; } = new List<CatalogProduct>();
        public int ClubLevel { get; set; }
        public bool CanPurchaseMultiple { get; set; }
        public bool IsPet { get; set; }
        public string PreviewImage { get; set; }

        public CatalogOffer() { }

        internal CatalogOffer(Packet packet)
        {
            Id = packet.ReadInteger();
            FurniLine = packet.ReadString();
            IsRentable = packet.ReadBoolean();
            PriceInCredits = packet.ReadInteger();
            PriceInActivityPoints = packet.ReadInteger();
            ActivityPointType = (ActivityPointType)packet.ReadInteger();
            CanPurchaseAsGift = packet.ReadBoolean();

            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                Products.Add(CatalogProduct.Parse(packet));

            ClubLevel = packet.ReadInteger();
            CanPurchaseMultiple = packet.ReadBoolean();
            IsPet = packet.ReadBoolean();
            PreviewImage = packet.ReadString();
        }
    }
}
