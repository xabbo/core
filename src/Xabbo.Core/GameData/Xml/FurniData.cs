using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Xabbo.Core.GameData.Xml;

[XmlRoot("furnidata")]
public class FurniData
{
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
    private static readonly XmlSerializer _serializer = new(typeof(FurniData));
    public static FurniData Load(Stream stream) => (FurniData?)_serializer.Deserialize(stream)
        ?? throw new Exception("Failed to deserialize furni data.");
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

    public static FurniData Load(string path)
    {
        using Stream stream = File.OpenRead(path);
        return Load(stream);
    }

    [XmlArray("roomitemtypes")]
    [XmlArrayItem("furnitype")]
    public List<FurniInfo> FloorItems { get; set; }

    [XmlArray("wallitemtypes")]
    [XmlArrayItem("furnitype")]
    public List<FurniInfo> WallItems { get; set; }

    public FurniData()
    {
        FloorItems = new List<FurniInfo>();
        WallItems = new List<FurniInfo>();
    }
}
