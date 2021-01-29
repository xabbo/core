using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class WallItem : Furni, IWallItem
    {
        public static WallItem Parse(IReadOnlyPacket packet, bool readName = true) => new WallItem(packet, readName);

        public static WallItem[] ParseAll(IReadOnlyPacket packet)
        {
            var ownerDictionary = new Dictionary<long, string>();

            int n = packet.ReadShort();
            for (int i = 0; i < n; i++)
                ownerDictionary.Add(packet.ReadLong(), packet.ReadString());

            n = packet.ReadShort();
            WallItem[] wallItems = new WallItem[n];
            for (int i = 0; i < n; i++)
            {
                var item = wallItems[i] = Parse(packet, false);
                if (ownerDictionary.TryGetValue(item.OwnerId, out string? ownerName))
                    item.OwnerName = ownerName;
            }

            return wallItems;
        }

        public static void WriteAll(IPacket packet, IEnumerable<IWallItem> items)
        {
            var ownerIds = new HashSet<long>();
            var ownerDictionary = items
                .Where(x => ownerIds.Add(x.OwnerId))
                .ToDictionary(
                    key => key.OwnerId,
                    val => val.OwnerName
                );

            packet.WriteInt(ownerDictionary.Count);
            foreach (var pair in ownerDictionary)
            {
                packet.WriteLong(pair.Key);
                packet.WriteString(pair.Value);
            }

            packet.WriteInt(items.Count());
            foreach (var item in items)
                item.Write(packet, false);
        }

        public static void WriteAll(IPacket packet, params WallItem[] items) => WriteAll(packet, (IEnumerable<IWallItem>)items);

        public override ItemType Type => ItemType.Wall;

        private string data;
        public string Data
        {
            get => data;
            set
            {
                if (value is null)
                    throw new ArgumentNullException("Data");
                data = value;
            }
        }

        public override int State => int.TryParse(Data, out int state) ? state : -1;

        public WallLocation Location { get; set; }

        [JsonIgnore]
        public int WallX => Location.WallX;

        [JsonIgnore]
        public int WallY => Location.WallY;

        [JsonIgnore]
        public int X => Location.X;

        [JsonIgnore]
        public int Y => Location.Y;

        [JsonIgnore]
        public WallOrientation Orientation => Location.Orientation;

        public WallItem()
        {
            OwnerId = -1;
            OwnerName = "(unknown)";

            Data = string.Empty;
            SecondsToExpiration = -1;
            Usage = FurniUsage.None;
            Location = WallLocation.Zero;
        }

        protected WallItem(IReadOnlyPacket packet, bool readName)
            : this()
        {
            Id = packet.ReadLong();
            Kind = packet.ReadInt();
            Location = WallLocation.Parse(packet.ReadString());
            Data = packet.ReadString();
            SecondsToExpiration = packet.ReadInt();
            Usage = (FurniUsage)packet.ReadInt();
            OwnerId = packet.ReadLong();

            if (readName && packet.CanReadString())
                OwnerName = packet.ReadString();
        }

        public override void Write(IPacket packet) => Write(packet, true);

        public override void Write(IPacket packet, bool writeOwnerName = true)
        {
            packet.WriteLong(Id);
            packet.WriteInt(Kind);
            packet.WriteString(Location.ToString());
            packet.WriteString(Data);
            packet.WriteInt(SecondsToExpiration);
            packet.WriteInt((int)Usage);
            packet.WriteLong(OwnerId);
            if (writeOwnerName)
                packet.WriteString(OwnerName);
        }
    }
}
