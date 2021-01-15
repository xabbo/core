using System;

namespace Xabbo.Core.Messages
{
    public class ReadOnlyPacket : IReadOnlyPacket
    {
        private readonly Packet _packet;

        public Header Header => _packet.Header;

        public int Position
        {
            get => _packet.Position;
            set => _packet.Position = value;
        }

        public int Length => _packet.Length;

        public int Available => _packet.Available;

        public ReadOnlyMemory<byte> GetBuffer() => _packet.GetBuffer();

        public void CopyTo(Span<byte> destination) => _packet.CopyTo(destination);

        public ReadOnlyPacket(Packet packet)
        {
            _packet = packet;
        }

        public bool CanReadBool() => _packet.CanReadBool();

        public bool CanReadString() => _packet.CanReadString();


        public bool ReadBool() => _packet.ReadBool();

        public bool ReadBool(int position) => _packet.ReadBool(position);

        public byte ReadByte() => _packet.ReadByte();

        public byte ReadByte(int position) => _packet.ReadByte(position);

        public void ReadBytes(Span<byte> buffer) => _packet.ReadBytes(buffer);

        public void ReadBytes(Span<byte> buffer, int position) => _packet.ReadBytes(buffer, position);

        public int ReadInt() => _packet.ReadInt();

        public int ReadInt(int position) => _packet.ReadInt(position);

        public float ReadFloat() => _packet.ReadFloat();

        public float ReadFloat(int position) => _packet.ReadFloat(position);

        public long ReadLong() => _packet.ReadLong();

        public long ReadLong(int position) => _packet.ReadLong(position);

        public short ReadShort() => _packet.ReadShort();

        public short ReadShort(int position) => _packet.ReadShort(position);

        public string ReadString() => _packet.ReadString();

        public string ReadString(int position) => _packet.ReadString(position);

        public float ReadFloatAsString() => _packet.ReadFloatAsString();

        public float ReadFloatAsString(int position) => _packet.ReadFloatAsString(position);
    }
}
