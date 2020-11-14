using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Xabbo.Core.Metadata
{
    [XmlRoot("furnidata")]
    public class FurniDataXml
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(FurniDataXml));
        public static FurniDataXml Load(Stream stream) => (FurniDataXml)_serializer.Deserialize(stream);
        public static FurniDataXml Load(string path)
        {
            using (Stream stream = File.OpenRead(path))
                return Load(stream);
        }

        [XmlArray("roomitemtypes")]
        [XmlArrayItem("furnitype")]
        public List<FurniInfo> FloorItems { get; set; }

        [XmlArray("wallitemtypes")]
        [XmlArrayItem("furnitype")]
        public List<FurniInfo> WallItems { get; set; }

        public FurniDataXml()
        {
            FloorItems = new List<FurniInfo>();
            WallItems = new List<FurniInfo>();
        }

        public class FurniInfo
        {
            [XmlAttribute("id")]
            public int Id { get; set; }

            [XmlAttribute("classname")]
            public string ClassName { get; set; }

            [XmlElement("revision")]
            public int Revision { get; set; }

            [XmlElement("defaultdir")]
            public int DefaultDirection { get; set; }

            [XmlElement("xdim")]
            public int XDimension { get; set; }

            [XmlElement("ydim")]
            public int YDimension { get; set; }

            [XmlArray("partcolors")]
            [XmlArrayItem("color")]
            public List<string> PartColors { get; set; }

            [XmlElement("name")]
            public string Name { get; set; }

            [XmlElement("description")]
            public string Description { get; set; }

            [XmlElement("adurl")]
            public string AdUrl { get; set; }

            [XmlElement("offerid")]
            public int OfferId { get; set; }

            [XmlElement("buyout")]
            public bool BuyOut { get; set; }

            [XmlElement("rentofferid")]
            public int RentOfferId { get; set; }

            [XmlElement("rentbuyout")]
            public bool RentBuyOut { get; set; }

            [XmlElement("bc")]
            public bool IsBuildersClub { get; set; }

            [XmlElement("excludeddynamic")]
            public bool ExcludedDynamic { get; set; }

            [XmlElement("customparams")]
            public string CustomParams { get; set; }

            [XmlElement("specialtype")]
            public int SpecialType { get; set; }

            [XmlElement("canstandon")]
            public bool CanStandOn { get; set; }

            [XmlElement("cansiton")]
            public bool CanSitOn { get; set; }

            [XmlElement("canlayon")]
            public bool CanLayOn { get; set; }

            [XmlElement("furniline")]
            public string FurniLine { get; set; }

            public FurniInfo()
            {
                PartColors = new List<string>();
            }
        }
    }
}
