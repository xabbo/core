using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Xabbo.Core.GameData.Xml;

[XmlRoot("figuredata")]
public class FigureData
{
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
    private static readonly XmlSerializer _serializer = new(typeof(FigureData));
    public static FigureData Load(Stream stream) => (FigureData?)_serializer.Deserialize(stream)
        ?? throw new Exception("Failed to deserialize figure data.");
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

    [XmlArray("colors")]
    [XmlArrayItem("palette")]
    public List<Palette> Palettes { get; set; } = [];

    [XmlArray("sets")]
    [XmlArrayItem("settype")]
    public List<PartSetCollection> SetCollections { get; set; } = [];

    public class Palette
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlElement("color")]
        public List<Color> Colors { get; set; } = [];
    }

    public class Color
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("index")]
        public int Index { get; set; }

        [XmlAttribute("club")]
        public int RequiredClubLevel { get; set; }

        [XmlAttribute("selectable")]
        public bool IsSelectable { get; set; }

        [XmlText]
        public string Value { get; set; } = "";
    }

    public class PartSetCollection
    {
        [XmlAttribute("type")]
        public string Type { get; set; } = "";

        [XmlAttribute("paletteid")]
        public int PaletteId { get; set; }

        [XmlAttribute]
        public int mand_m_0 { get; set; }

        [XmlAttribute]
        public int mand_f_0 { get; set; }

        [XmlAttribute]
        public int mand_m_1 { get; set; }

        [XmlAttribute]
        public int mand_f_1 { get; set; }

        [XmlElement("set")]
        public List<PartSet> Sets { get; set; } = new();
    }

    public class PartSet
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("gender")]
        public string Gender { get; set; } = "";

        [XmlAttribute("club")]
        public int RequiredClubLevel { get; set; }

        [XmlAttribute("colorable")]
        public bool IsColorable { get; set; }

        [XmlAttribute("selectable")]
        public bool IsSelectable { get; set; }

        [XmlAttribute("preselectable")]
        public bool IsPreSelectable { get; set; }

        [XmlAttribute("sellable")]
        public bool IsSellable { get; set; }

        [XmlElement("part")]
        public List<Part> Parts { get; set; } = [];
    }

    public class Part
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; } = "";

        [XmlAttribute("colorable")]
        public bool IsColorable { get; set; }

        [XmlAttribute("index")]
        public int Index { get; set; }

        [XmlAttribute("colorindex")]
        public int ColorIndex { get; set; }
    }
}
