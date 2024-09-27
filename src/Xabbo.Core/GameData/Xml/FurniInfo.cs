using System.Collections.Generic;
using System.Xml.Serialization;

namespace Xabbo.Core.GameData.Xml;

public class FurniInfo
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("classname")]
    public string? ClassName { get; set; }

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
    public List<string> PartColors { get; set; } = new();

    [XmlElement("name")]
    public string? Name { get; set; }

    [XmlElement("description")]
    public string? Description { get; set; }

    [XmlElement("adurl")]
    public string? AdUrl { get; set; }

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
    public string? CustomParams { get; set; }

    [XmlElement("specialtype")]
    public int SpecialType { get; set; }

    [XmlElement("canstandon")]
    public bool CanStandOn { get; set; }

    [XmlElement("cansiton")]
    public bool CanSitOn { get; set; }

    [XmlElement("canlayon")]
    public bool CanLayOn { get; set; }

    [XmlElement("furniline")]
    public string? FurniLine { get; set; }
}
