using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core.Messages
{
    public class InterceptEventArgs : EventArgs
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

        private Packet packet;
        public Packet Packet
        {
            get => packet;
            set
            {
                if (value == null) throw new ArgumentNullException("Packet");
                if (ReferenceEquals(packet, value)) return;
                packet = value;
            }
        }

        public InterceptEventArgs(Destination destination, Packet packet, int step)
        {
            Time = DateTime.Now;

            Destination = destination;
            this.packet = packet;

            originalHeader = packet.Header;
            originalData = packet.GetData();

            Step = step;
        }

        public void Block() => IsBlocked = true;

        public Packet GetOriginalPacket()
        {
            return new Packet(originalData, false) { Header = originalHeader };
        }
    }
}
