using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core.Messages
{
    public class InterceptEventArgs : EventArgs
    {
        private readonly short originalHeader = -1;
        private readonly byte[] originalData;

        public Destination Destination { get; }
        public bool IsOutgoing => Destination == Destination.Server;
        public bool IsIncoming => Destination == Destination.Client;

        public bool IsBlocked { get; private set; }

        // TODO packet can be written to without accessing the setter, so this doesn't work ...
        public bool IsModified
        {
            get
            {
                if (packet.Header != originalHeader) return true;
                if (packet.Length != originalData.Length) return true;
                
                // TODO Hashing packet content ?

                byte[] newData = packet.ToBytes();
                for (int i = 0; i < originalData.Length; i++)
                {
                    if (originalData[i] != newData[i])
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

        public InterceptEventArgs(Destination destination, Packet packet)
        {
            Destination = destination;
            this.packet = packet;

            originalHeader = packet.Header;
            originalData = packet.GetData();
        }

        public void Block() => IsBlocked = true;

        public Packet GetOriginalPacket()
        {
            return new Packet(originalData, false) { Header = originalHeader };
        }
    }
}
