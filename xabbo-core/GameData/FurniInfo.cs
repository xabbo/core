using System;
using System.Collections.Generic;
using System.Linq;

namespace Xabbo.Core.GameData
{
    public class FurniInfo
    {
        /// <summary>
        /// Gets the type of the furni.
        /// </summary>
        public ItemType Type { get; }
        /// <summary>
        /// Gets the kind of the furni.
        /// </summary>
        public int Kind { get; }
        /// <summary>
        /// Gets the unique identifier string of the furni.
        /// </summary>
        public string Identifier { get; }
        public int Revision { get; }
        public int DefaultDirection { get; }
        public int XDimension { get; }
        public int YDimension { get; }
        public IReadOnlyList<string> PartColors { get; }
        /// <summary>
        /// Gets the name of the furni.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the description of the furni.
        /// </summary>
        public string Description { get; }
        public string AdUrl { get; }
        public int OfferId { get; }
        public bool BuyOut { get; }
        public int RentOfferId { get; }
        public bool RentBuyOut { get; }
        public bool IsBuildersClub { get; }
        public bool ExcludedDynamic { get; }
        public string CustomParams { get; }
        /// <summary>
        /// Gets the category of the furni.
        /// </summary>
        public FurniCategory Category { get; }
        public bool CanStandOn { get; }
        public bool CanSitOn { get; }
        public bool CanLayOn { get; }
        public string Line { get; }
        public bool IsRare { get; }

        internal FurniInfo(ItemType type, Xml.FurniInfo proxy)
        {
            Type = type;
            Kind = proxy.Id;
            Identifier = proxy.ClassName;
            Revision = proxy.Revision;
            DefaultDirection = proxy.DefaultDirection;
            XDimension = proxy.XDimension;
            YDimension = proxy.YDimension;
            PartColors = proxy.PartColors.AsReadOnly();
            Name = proxy.Name;
            Description = proxy.Description;
            AdUrl = proxy.AdUrl;
            OfferId = proxy.OfferId;
            BuyOut = proxy.BuyOut;
            RentOfferId = proxy.RentOfferId;
            RentBuyOut = proxy.RentBuyOut;
            IsBuildersClub = proxy.IsBuildersClub;
            ExcludedDynamic = proxy.ExcludedDynamic;
            CustomParams = proxy.CustomParams;
            Category = (FurniCategory)proxy.SpecialType;
            CanStandOn = proxy.CanStandOn;
            CanSitOn = proxy.CanSitOn;
            CanLayOn = proxy.CanLayOn;
            Line = proxy.FurniLine;
        }

        internal FurniInfo(ItemType type, Json.FurniInfo proxy)
        {
            Type = type;
            Kind = proxy.Id;
            Identifier = proxy.ClassName;
            Revision = proxy.Revision;
            DefaultDirection = proxy.DefaultDir;
            XDimension = proxy.XDim;
            YDimension = proxy.YDim;
            PartColors = proxy.PartColors?.Colors.ToList().AsReadOnly();
            Name = proxy.Name;
            Description = proxy.Description;
            AdUrl = proxy.AdUrl;
            OfferId = proxy.OfferId;
            BuyOut = proxy.Buyout;
            RentOfferId = proxy.RentOfferId;
            RentBuyOut = proxy.RentBuyout;
            IsBuildersClub = proxy.BC;
            ExcludedDynamic = proxy.ExcludedDynamic;
            CustomParams = proxy.CustomParams;
            Category = (FurniCategory)proxy.SpecialType;
            CanStandOn = proxy.CanStandOn;
            CanSitOn = proxy.CanSitOn;
            CanLayOn = proxy.CanLayOn;
            Line = proxy.FurniLine;
            IsRare = proxy.Rare;
        }

        /// <summary>
        /// Returns <c>true</c> if <see cref="CanStandOn"/>, <see cref="CanSitOn"/> and <see cref="CanLayOn"/> are all <c>false</c>.
        /// </summary>
        public bool IsUnwalkable => !CanStandOn && !CanSitOn && !CanLayOn;
    }
}
