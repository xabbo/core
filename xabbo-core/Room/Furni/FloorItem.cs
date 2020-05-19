using System;
using System.Collections.Generic;
using System.Globalization;

using Xabbo.Core.Protocol;

using Newtonsoft.Json;

namespace Xabbo.Core
{
    public class FloorItem : Furni
    {
        public static FloorItem Parse(Packet packet, bool readName = true) => new FloorItem(packet, readName);
        public static FloorItem ParseUpdate(Packet packet) => new FloorItem(packet, false);

        public static FloorItem[] ParseAll(Packet packet)
        {
            var ownerDictionary = new Dictionary<int, string>();

            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                ownerDictionary.Add(packet.ReadInteger(), packet.ReadString());

            n = packet.ReadInteger();
            var items = new FloorItem[n];
            for (int i = 0; i < n; i++)
            {
                var item = items[i] = Parse(packet, false);
                if (ownerDictionary.TryGetValue(item.OwnerId, out string ownerName))
                    item.OwnerName = ownerName;
            }

            return items;
        }

        public override FurniType Type => FurniType.Floor;

        public Tile Tile { get; set; }
        [JsonIgnore] public int X => Tile.X;
        [JsonIgnore] public int Y => Tile.Y;
        [JsonIgnore] public double Z => Tile.Z;
        public int Direction { get; set; }
        public double Height { get; set; }
        public int Extra { get; set; }

        private StuffData _data;
        public StuffData Data
        {
            get => _data;
            set
            {
                _data = value;

                if (!string.IsNullOrWhiteSpace(value?.LegacyString) &&
                    double.TryParse(value.LegacyString, out double state))
                {
                    State = (int)state;
                }
                else
                    State = 0;
            }
        }
        [JsonIgnore] public int State { get; private set; }

        public int SecondsToExpiration { get; set; }
        public FurniUsage Usage { get; set; }

        public string UnknownStringA { get; set; }

        public FloorItem() { }

        private FloorItem(Packet packet, bool readName)
        {
            Id = packet.ReadInteger();
            Kind = packet.ReadInteger();
            int x = packet.ReadInteger();
            int y = packet.ReadInteger();
            Direction = packet.ReadInteger();
            double z = packet.ReadDouble();
            Tile = new Tile(x, y, z);
            string heightString = packet.ReadString();
            Height = double.Parse(heightString, CultureInfo.InvariantCulture);
            Extra = packet.ReadInteger();
            // - consumable state e.g. cabbage 0: full, 1: partly eaten, 2: mostly eaten
            // - linked teleport id

            Data = StuffData.Parse(packet);

            SecondsToExpiration = packet.ReadInteger();
            Usage = (FurniUsage)packet.ReadInteger();
            OwnerId = packet.ReadInteger();

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
                Tile.X,
                Tile.Y,
                Direction,
                Tile.Z,
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
