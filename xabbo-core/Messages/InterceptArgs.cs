using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core.Messages
{
    public class InterceptArgs : EventArgs
    {
        private readonly short originalHeader = -1;
        private readonly byte[] originalData;

        public DateTime Time { get; }
        public int Step { get; }
        public Destination Destination { get; }
        public bool IsOutgoing => Destination == Destination.Server;
        public bool IsIncoming => Destination == Destination.Client;

        public bool IsBlocked { get; private set; }

        public bool IsModified
        {
            get
            {
                if (packet.Header != originalHeader) return true;
                if (packet.Length != originalData.Length) return true;

                byte[] data = packet.GetData();
                for (int i = 0; i < originalData.Length; i++)
                {
                    if (originalData[i] != data[i])
                        return true;
                }

                return false;
            }
        }

        private IPacket packet;
        public IPacket Packet
        {
            get => packet;
            set
            {
                if (value == null) throw new ArgumentNullException("Packet");
                if (ReferenceEquals(packet, value)) return;
                packet = value;
            }
        }

        public InterceptArgs(Destination destination, IPacket packet, int step)
        {
            Time = DateTime.Now;

            Destination = destination;
            this.packet = packet;

            originalHeader = packet.Header;
            originalData = packet.GetData();

            Step = step;
        }

        public void Block() => IsBlocked = true;

        public IPacket GetOriginalPacket()
        {
            return new Packet(originalData, false) { Header = originalHeader };
        }
    }
}
