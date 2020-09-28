using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class FloorItem : Furni, IFloorItem
    {
        public static FloorItem Parse(Packet packet, bool readName = true) => new FloorItem(packet, readName);
        public static FloorItem ParseUpdate(Packet packet) => new FloorItem(packet, false);

        public static FloorItem[] ParseAll(Packet packet)
        {
            var ownerDictionary = new Dictionary<int, string>();

            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
                ownerDictionary.Add(packet.ReadInt(), packet.ReadString());

            n = packet.ReadInt();
            var items = new FloorItem[n];
            for (int i = 0; i < n; i++)
            {
                var item = items[i] = Parse(packet, false);
                if (ownerDictionary.TryGetValue(item.OwnerId, out string ownerName))
                    item.OwnerName = ownerName;
            }

            return items;
        }

        public override ItemType Type => ItemType.Floor;

        public Tile Location { get; set; }
        ITile IFloorItem.Location => Location;
        [JsonIgnore] public int X => Location.X;
        [JsonIgnore] public int Y => Location.Y;
        [JsonIgnore] public double Z => Location.Z;
        public int Direction { get; set; }
        public double Height { get; set; }
        public int Extra { get; set; }

        private ItemData _data;
        public ItemData Data
        {
            get => _data;
            set
            {
                _data = value;

                if (!string.IsNullOrWhiteSpace(_data?.LegacyString) &&
                    double.TryParse(_data.LegacyString, out double state))
                {
                    State = (int)state;
                }
                else
                    State = -1;
            }
        }
        IItemData IFloorItem.Data => Data;

        public string UnknownStringA { get; set; }

        public FloorItem()
        {
            Location = new Tile();
            Data = new LegacyData();
            SecondsToExpiration = -1;
            Usage = FurniUsage.None;
        }

        private FloorItem(Packet packet, bool readName)
        {
            Id = packet.ReadInt();
            Kind = packet.ReadInt();
            int x = packet.ReadInt();
            int y = packet.ReadInt();
            Direction = packet.ReadInt();
            double z = packet.ReadDouble();
            Location = new Tile(x, y, z);
            Height = packet.ReadDouble();
            Extra = packet.ReadInt();
            // - consumable state e.g. cabbage 0: full, 1: partly eaten, 2: mostly eaten
            // - linked teleport id

            Data = ItemData.Parse(packet);

            SecondsToExpiration = packet.ReadInt();
            Usage = (FurniUsage)packet.ReadInt();
            OwnerId = packet.ReadInt();

            if (Kind < 0) // ?
                UnknownStringA = packet.ReadString();

            if (readName && packet.CanReadString())
                OwnerName = packet.ReadString();
        }

        public override void Write(Packet packet) => Write(packet, true);

        public override void Write(Packet packet, bool writeName)
        {
            packet.WriteValues(
                Id,
                Kind,
                Location.X,
                Location.Y,
                Direction,
                Location.Z,
                Height,
                Extra,
                Data,
                SecondsToExpiration,
                (int)Usage,
                OwnerId
            );
            if (Kind < 0) packet.WriteString(UnknownStringA);
            if (writeName) packet.WriteString(OwnerName);
        }
    }
}
