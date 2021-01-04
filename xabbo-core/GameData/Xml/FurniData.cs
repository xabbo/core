using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Xabbo.Core.GameData.Xml
{
    [XmlRoot("furnidata")]
    public class FurniData
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(FurniData));
        public static FurniData Load(Stream stream) => (FurniData)_serializer.Deserialize(stream);
        public static FurniData Load(string path)
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

        public FurniData()
        {
            FloorItems = new List<FurniInfo>();
            WallItems = new List<FurniInfo>();
        }
    }
}
