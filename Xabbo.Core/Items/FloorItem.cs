using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class FloorItem : Furni, IFloorItem
    {
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

        public ItemData Data { get; set; }
        IItemData IFloorItem.Data => Data;

        public override int State => double.TryParse(Data.Value, out double state) ? (int)state : -1;

        public string StaticClass { get; set; }

        public FloorItem()
        {
            Location = Tile.Zero;
            Data = new LegacyData();
            SecondsToExpiration = -1;
            Usage = FurniUsage.None;
            StaticClass = string.Empty;
        }

        public FloorItem(IFloorItem item)
        {
            Id = item.Id;
            Kind = item.Kind;
            Location = item.Location;
            Direction = item.Direction;
            Height = item.Height;
            Extra = item.Extra;
            Data = ItemData.Clone(item.Data);
            SecondsToExpiration = item.SecondsToExpiration;
            Usage = item.Usage;
            OwnerId = item.OwnerId;
            OwnerName = item.OwnerName;
            StaticClass = item.StaticClass;
        }

        protected FloorItem(IReadOnlyPacket packet, bool readName)
        {
            Id = packet.ReadLegacyLong();
            Kind = packet.ReadInt();
            int x = packet.ReadInt();
            int y = packet.ReadInt();
            Direction = packet.ReadInt();
            float z = packet.ReadLegacyFloat();
            Location = new Tile(x, y, z);
            Height = packet.ReadLegacyFloat();
            Extra = packet.ReadLegacyLong();
            // - consumable state e.g. cabbage 0: full, 1: partly eaten, 2: mostly eaten
            // - linked teleport id

            Data = ItemData.Parse(packet);

            SecondsToExpiration = packet.ReadInt();
            Usage = (FurniUsage)packet.ReadInt();
            OwnerId = packet.ReadLegacyLong();

            if (Kind < 0)
            {
                StaticClass = packet.ReadString();
            }
            else
            {
                StaticClass = string.Empty;
            }

            if (readName && packet.CanReadString())
                OwnerName = packet.ReadString();
        }

        public override void Compose(IPacket packet)
        {
            Compose(packet, true);
        }

        public override void Compose(IPacket packet, bool writeOwnerName = true)
        {
            packet
                .WriteLegacyLong(Id)
                .WriteInt(Kind)
                .WriteInt(Location.X)
                .WriteInt(Location.Y)
                .WriteInt(Direction)
                .WriteLegacyFloat(Location.Z)
                .WriteLegacyFloat(Height)
                .WriteLegacyLong(Extra)
                .Write(Data)
                .WriteInt(SecondsToExpiration)
                .WriteInt((int)Usage)
                .WriteLegacyLong(OwnerId);

            if (Kind < 0) packet.WriteString(StaticClass);
            if (writeOwnerName) packet.WriteString(OwnerName);
        }
        

        public static FloorItem Parse(IReadOnlyPacket packet, bool readName = true)
        {
            return new FloorItem(packet, readName);
        }

        public static FloorItem ParseUpdate(IReadOnlyPacket packet)
        {
            return new FloorItem(packet, false);
        }

        public static FloorItem[] ParseAll(IReadOnlyPacket packet)
        {
            var ownerDictionary = new Dictionary<long, string>();

            short n = packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
                ownerDictionary.Add(packet.ReadLegacyLong(), packet.ReadString());

            n = packet.ReadLegacyShort();
            var items = new FloorItem[n];
            for (int i = 0; i < n; i++)
            {
                var item = items[i] = Parse(packet, false);
                if (ownerDictionary.TryGetValue(item.OwnerId, out string? ownerName))
                    item.OwnerName = ownerName;
            }

            return items;
        }

        public static void ComposeAll(IPacket packet, IEnumerable<IFloorItem> items)
        {
            var ownerIds = new HashSet<long>();
            var ownerDictionary = items
                .Where(x => ownerIds.Add(x.OwnerId))
                .ToDictionary(
                    key => key.OwnerId,
                    val => val.OwnerName
                );

            packet.WriteLegacyShort((short)ownerDictionary.Count);
            foreach (var pair in ownerDictionary)
            {
                packet.WriteLegacyLong(pair.Key);
                packet.WriteString(pair.Value);
            }

            packet.WriteLegacyShort((short)items.Count());
            foreach (IFloorItem item in items)
                item.Compose(packet, false);
        }

        public static void ComposeAll(IPacket packet, params FloorItem[] items)
        {
            ComposeAll(packet, (IEnumerable<IFloorItem>)items);
        }

    }
}
