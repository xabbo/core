using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Xabbo.Messages;

namespace Xabbo.Core
{
    public class WallItem : Furni, IWallItem
    {
        public override ItemType Type => ItemType.Wall;

        private string _data;
        public string Data
        {
            get => _data;
            set
            {
                if (value is null)
                    throw new ArgumentNullException("Data");
                _data = value;
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

            _data = string.Empty;
            SecondsToExpiration = -1;
            Usage = FurniUsage.None;
            Location = WallLocation.Zero;
        }

        protected WallItem(IReadOnlyPacket packet, bool readName)
            : this()
        {
            Id = packet.Protocol switch
            {
                ClientType.Flash => long.Parse(packet.ReadString()),
                ClientType.Unity => packet.ReadLong(),
                _ => throw new InvalidOperationException("Unknown protocol.")
            };
            Kind = packet.ReadInt();
            Location = WallLocation.Parse(packet.ReadString());
            Data = packet.ReadString();
            SecondsToExpiration = packet.ReadInt();
            Usage = (FurniUsage)packet.ReadInt();
            OwnerId = packet.ReadLegacyLong();

            if (readName && packet.CanReadString())
                OwnerName = packet.ReadString();
        }

        public override void Compose(IPacket packet)
        {
            Compose(packet, true);
        }

        public override void Compose(IPacket packet, bool writeOwnerName = true)
        {
            if (packet.Protocol == ClientType.Flash) packet.WriteString(Id.ToString());
            else if (packet.Protocol == ClientType.Unity) packet.WriteLong(Id);
            else throw new InvalidOperationException("Unknown protocol");

            packet.WriteInt(Kind);
            packet.WriteString(Location.ToString());
            packet.WriteString(Data);
            packet.WriteInt(SecondsToExpiration);
            packet.WriteInt((int)Usage);
            packet.WriteLegacyLong(OwnerId);

            if (writeOwnerName)
            {
                packet.WriteString(OwnerName);
            }
        }

        public static WallItem Parse(IReadOnlyPacket packet, bool readName = true)
        {
            return new WallItem(packet, readName);
        }

        public static WallItem[] ParseAll(IReadOnlyPacket packet)
        {
            var ownerDictionary = new Dictionary<long, string>();

            int n = packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
                ownerDictionary.Add(packet.ReadLegacyLong(), packet.ReadString());

            n = packet.ReadLegacyShort();
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

            packet.WriteLegacyShort((short)ownerDictionary.Count);
            foreach (var pair in ownerDictionary)
            {
                packet.WriteLegacyLong(pair.Key);
                packet.WriteString(pair.Value);
            }

            packet.WriteLegacyShort((short)items.Count());
            foreach (var item in items)
                item.Compose(packet, false);
        }

        public static void WriteAll(IPacket packet, params WallItem[] items)
        {
            WriteAll(packet, (IEnumerable<IWallItem>)items);
        }

    }
}
