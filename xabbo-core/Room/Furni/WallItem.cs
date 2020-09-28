using System;
using System.Collections.Generic;
using Xabbo.Core.Protocol;

using Newtonsoft.Json;

namespace Xabbo.Core
{
    public class WallItem : Furni, IWallItem
    {
        public static WallItem Parse(Packet packet, bool readName = true) => new WallItem(packet, readName);

        public static WallItem[] ParseAll(Packet packet)
        {
            var ownerDictionary = new Dictionary<int, string>();

            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
                ownerDictionary.Add(packet.ReadInt(), packet.ReadString());

            n = packet.ReadInt();
            var wallItems = new WallItem[n];
            for (int i = 0; i < n; i++)
            {
                var item = wallItems[i] = Parse(packet, false);
                if (ownerDictionary.TryGetValue(item.OwnerId, out string ownerName))
                    item.OwnerName = ownerName;
            }

            return wallItems;
        }

        public override ItemType Type => ItemType.Wall;

        private string _data;
        public string Data
        {
            get => _data;
            set
            {
                _data = value;

                if (!string.IsNullOrWhiteSpace(_data) && int.TryParse(_data, out int state))
                    State = state;
                else
                    State = -1;
            }
        }

        public WallLocation Location { get; set; }
        IWallLocation IWallItem.Location => Location;

        [JsonIgnore]
        public int WallX
        {
            get => Location.WallX;
            set => Location.WallX = value;
        }

        [JsonIgnore]
        public int WallY
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

        [JsonIgnore]
        public int Y
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
            OwnerId = -1;
            OwnerName = "(unknown)";

            Data = "";
            SecondsToExpiration = -1;
            Usage = FurniUsage.None;
            Location = WallLocation.Zero;
        }

        internal WallItem(Packet packet, bool readName)
            : this()
        {
            string idString = packet.ReadString();
            if (!int.TryParse(idString, out int id))
                throw new FormatException($"Unable to parse wall item id: '{idString}'");

            Id = id;
            Kind = packet.ReadInt();
            Location = WallLocation.Parse(packet.ReadString());
            Data = packet.ReadString();
            SecondsToExpiration = packet.ReadInt();
            Usage = (FurniUsage)packet.ReadInt();
            OwnerId = packet.ReadInt();

            if (readName && packet.CanReadString())
                OwnerName = packet.ReadString();
        }

        public override void Write(Packet packet) => Write(packet, true);

        public override void Write(Packet packet, bool writeName = true)
        {
            packet.WriteString(Id.ToString());
            packet.WriteInt(Kind);
            packet.WriteString(Location.ToString());
            packet.WriteString(Data);
            packet.WriteInt(SecondsToExpiration);
            packet.WriteInt((int)Usage);
            packet.WriteInt(OwnerId);
            if (writeName)
                packet.WriteString(OwnerName);
        }
    }
}
