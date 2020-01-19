using System;
using System.Collections.Generic;
using Xabbo.Core.Protocol;

using Newtonsoft.Json;

namespace Xabbo.Core
{
    public class WallItem : Furni
    {
        public static WallItem Parse(Packet packet, bool readName = true) => new WallItem(packet, readName);

        public static WallItem[] ParseAll(Packet packet)
        {
            var ownerDictionary = new Dictionary<int, string>();

            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                ownerDictionary.Add(packet.ReadInteger(), packet.ReadString());

            n = packet.ReadInteger();
            var wallItems = new WallItem[n];
            for (int i = 0; i < n; i++)
            {
                var item = wallItems[i] = Parse(packet, false);
                if (ownerDictionary.TryGetValue(item.OwnerId, out string ownerName))
                    item.OwnerName = ownerName;
            }

            return wallItems;
        }

        public override FurniType Type => FurniType.Wall;

        public string Data { get; set; }
        public int SecondsToExpiration { get; set; }
        public int UnknownIntA { get; set; }

        public WallLocation Location { get; set; }

        [JsonIgnore] public int WallX
        {
            get => Location.WallX;
            set => Location.WallX = value;
        }

        [JsonIgnore] public int WallY
        {
            get => Location.WallY;
            set => Location.WallY = value;
        }

        [JsonIgnore]
        public int X
        {
            get => Location.X;
            set => Location.X = value;
        }

        [JsonIgnore] public int Y
        {
            get => Location.Y;
            set => Location.Y = value;
        }

        [JsonIgnore]
        public WallOrientation Orientation
        {
            get => Location.Orientation;
            set => Location.Orientation = value;
        }

        public WallItem()
        {
            OwnerName = "(unknown)";
        }

        internal WallItem(Packet packet, bool readName)
            : this()
        {
            string idString = packet.ReadString();
            if (!int.TryParse(idString, out int id))
                throw new FormatException($"Unable to parse wall item id: '{idString}'");

            Id = id;
            Kind = packet.ReadInteger();
            Location = WallLocation.Parse(packet.ReadString());
            Data = packet.ReadString();
            SecondsToExpiration = packet.ReadInteger();
            UnknownIntA = packet.ReadInteger();
            OwnerId = packet.ReadInteger();

            if (readName && packet.CanReadString())
                OwnerName = packet.ReadString();
        }

        public override void Write(Packet packet) => Write(packet, true);

        public override void Write(Packet packet, bool writeName = true)
        {
            packet.WriteString(Id.ToString());
            packet.WriteInteger(Kind);
            packet.WriteString(Location.ToString());
            packet.WriteString(Data);
            packet.WriteInteger(SecondsToExpiration);
            packet.WriteInteger(UnknownIntA);
            packet.WriteInteger(OwnerId);
            if (writeName) packet.WriteString(OwnerName);
        }
    }
}
