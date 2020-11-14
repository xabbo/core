using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class CrackableFurniData : ItemData, ICrackableFurniData
    {
        public int Hits { get; set; }
        public int Target { get; set; }

        public CrackableFurniData()
            : base(ItemDataType.CrackableFurni)
        { }

        protected override void Initialize(IReadOnlyPacket packet)
        {
            LegacyString = packet.ReadString();
            Hits = packet.ReadInt();
            Target = packet.ReadInt();

            base.Initialize(packet);
        }

        protected override void WriteData(IPacket packet)
        {
            packet.WriteString(LegacyString);
            packet.WriteInt(Hits);
            packet.WriteInt(Target);

            WriteBase(packet);
        }
    }
}
