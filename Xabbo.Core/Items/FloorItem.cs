using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    public class FloorItem : Furni, IFloorItem
    {
        public static FloorItem Parse(IReadOnlyPacket packet, bool readName = true) => new FloorItem(packet, readName);
        public static FloorItem ParseUpdate(IReadOnlyPacket packet) => new FloorItem(packet, false);

        public static FloorItem[] ParseAll(IReadOnlyPacket packet)
        {
            var ownerDictionary = new Dictionary<long, string>();

            int n = packet.ReadShort();
            for (int i = 0; i < n; i++)
                ownerDictionary.Add(packet.ReadLong(), packet.ReadString());

            n = packet.ReadShort();
            var items = new FloorItem[n];
            for (int i = 0; i < n; i++)
            {
                var item = items[i] = Parse(packet, false);
                if (ownerDictionary.TryGetValue(item.OwnerId, out string? ownerName))
                    item.OwnerName = ownerName;
            }

            return items;
        }

        public static void WriteAll(IPacket packet, IEnumerable<IFloorItem> items)
        {
            var ownerIds = new HashSet<long>();
            var ownerDictionary = items
                .Where(x => ownerIds.Add(x.OwnerId))
                .ToDictionary(
                    key => key.OwnerId,
                    val => val.OwnerName
                );

            packet.WriteShort((short)ownerDictionary.Count);
            foreach (var pair in ownerDictionary)
            {
                packet.WriteLong(pair.Key);
                packet.WriteString(pair.Value);
            }

            packet.WriteShort((short)items.Count());
            foreach (var item in items)
                item.Write(packet, false);
        }

        public static void WriteAll(IPacket packet, params FloorItem[] items) => WriteAll(packet, (IEnumerable<IFloorItem>)items);

        public override ItemType Type => ItemType.Floor;

        public Tile Location { get; set; }
        [JsonIgnore] public int X => Location.X;
        [JsonIgnore] public int Y => Location.Y;
        [JsonIgnore] public (int X, int Y) XY => Location.XY;
        [JsonIgnore] public double Z => Location.Z;
        [JsonIgnore] public (int X, int Y, double Z) XYZ => Location.XYZ;
        public int Direction { get; set; }
        public float Height { get; set; }
        public long Extra { get; set; }

        public StuffData Data { get; set; }
        IItemData IFloorItem.Data => Data;

        public override int State => double.TryParse(Data.Value, out double state) ? (int)state : -1;

        public string UnknownStringA { get; set; }

        public FloorItem()
        {
            Location = Tile.Zero;
            Data = new LegacyData();
            SecondsToExpiration = -1;
            Usage = FurniUsage.None;
            UnknownStringA = string.Empty;
        }

        protected FloorItem(IReadOnlyPacket packet, bool readName)
        {
            Id = packet.ReadLong();
            Kind = packet.ReadInt();
            int x = packet.ReadInt();
            int y = packet.ReadInt();
            Direction = packet.ReadInt();
            float z = packet.ReadFloat();
            Location = new Tile(x, y, z);
            Height = packet.ReadFloat();
            Extra = packet.ReadLong();
            // - consumable state e.g. cabbage 0: full, 1: partly eaten, 2: mostly eaten
            // - linked teleport id

            Data = StuffData.Parse(packet);

            SecondsToExpiration = packet.ReadInt();
            Usage = (FurniUsage)packet.ReadInt();
            OwnerId = packet.ReadLong();

            if (Kind < 0) // ?
                UnknownStringA = packet.ReadString();
            else
                UnknownStringA = string.Empty;

            if (readName && packet.CanReadString())
                OwnerName = packet.ReadString();
        }

        public override void Write(IPacket packet) => Write(packet, true);

        public override void Write(IPacket packet, bool writeOwnerName = true)
        {
            packet.WriteLong(Id);
            packet.WriteInt(Kind);
            packet.WriteInt(Location.X);
            packet.WriteInt(Location.Y);
            packet.WriteInt(Direction);
            packet.WriteFloat(Location.Z);
            packet.WriteFloat(Height);
            packet.WriteLong(Extra);
            Data.Write(packet);
            packet.WriteInt(SecondsToExpiration);
            packet.WriteInt((int)Usage);
            packet.WriteLong(OwnerId);

            if (Kind < 0) packet.WriteString(UnknownStringA);
            if (writeOwnerName) packet.WriteString(OwnerName);
        }
    }
}
