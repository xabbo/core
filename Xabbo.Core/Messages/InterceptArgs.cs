using System;
using System.Buffers;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Messages
{
    public class InterceptArgs : EventArgs, IDisposable
    {
        private readonly Header _originalHeader = Header.Unknown;
        private readonly IMemoryOwner<byte> _originalDataOwner;
        private readonly ReadOnlyMemory<byte> _originalData;

        public DateTime Time { get; }
        public int Step { get; }
        public Destination Destination { get; }
        public bool IsOutgoing => Destination == Destination.Server;
        public bool IsIncoming => Destination == Destination.Client;

        public bool IsBlocked { get; private set; }

        public bool IsModified => 
            packet.Header != _originalHeader ||
            packet.Length != _originalData.Length ||
            !packet.GetBuffer().Span.SequenceEqual(_originalData.Span);

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

            _originalHeader = packet.Header;
            _originalDataOwner = MemoryPool<byte>.Shared.Rent(packet.Length);
            _originalData = _originalDataOwner.Memory[0..packet.Length];

            packet.CopyTo(_originalDataOwner.Memory.Span);

            Step = step;
        }

        public void Block() => IsBlocked = true;

        public IPacket GetOriginalPacket()
        {
            return new Packet(_originalData.Span) { Header = _originalHeader };
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _originalDataOwner.Dispose();
            }
        }
    }
}
