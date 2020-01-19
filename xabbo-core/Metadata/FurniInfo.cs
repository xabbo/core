using System;
using System.Collections.Generic;

namespace Xabbo.Core.Metadata
{
    public class FurniInfo
    {
        public FurniType Type { get; }
        public int Id { get; }
        public string ClassName { get; }
        public int Revision { get; }
        public int DefaultDirection { get; }
        public int XDimension { get; }
        public int YDimension { get; }
        public IReadOnlyList<string> PartColors { get; }
        public string Name { get; }
        public string Description { get; }
        public string AdUrl { get; }
        public int OfferId { get; }
        public bool BuyOut { get; }
        public int RentOfferId { get; }
        public bool RentBuyOut { get; }
        public bool IsBuildersClub { get; }
        public bool ExcludedDynamic { get; }
        public string CustomParams { get; }
        public FurniCategory Category { get; }
        public bool CanStandOn { get; }
        public bool CanSitOn { get; }
        public bool CanLayOn { get; }
        public string Line { get; }

        internal FurniInfo(FurniType type, FurniDataXml.FurniInfo proxy)
        {
            Type = type;
            Id = proxy.Id;
            ClassName = proxy.ClassName;
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

        public bool IsUnwalkable => !CanStandOn && !CanSitOn && !CanLayOn;
    }
}
