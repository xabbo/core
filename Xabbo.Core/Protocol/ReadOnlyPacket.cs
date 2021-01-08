using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using Xabbo.Core.Messages;

namespace Xabbo.Core.Protocol
{
    /*public class ReadOnlyPacket : IReadOnlyPacket
    {
        private readonly ReadOnlyMemory<byte> _buffer;

        public Header Header { get; }
        public int Length { get; }

        private int _position;
        public int Position
        {
            get => _position;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(Position));
                if (value > Length) throw new ArgumentOutOfRangeException(nameof(Position));
                _position = value;
            }
        }

        public int Available => Length - Position;

        public ReadOnlyPacket(Header header, ReadOnlyMemory<byte> buffer)
        {
            Header = header;

            _buffer = buffer;

            Length = _buffer.Length;
            Position = 0;
        }

        public ReadOnlyMemory<byte> GetBuffer() => _buffer;

        private ReadOnlySpan<byte> Slice(int position, int length)
        {
            if (position < 0 || position >= Length)
                throw new ArgumentOutOfRangeException("position");
            if (length < 0 || (position + length) > Length)
                throw new ArgumentOutOfRangeException("length");
            return _buffer.Span.Slice(position, length);
        }

        private ReadOnlySpan<byte> SliceAndAdvance(int length)
        {
            if ((Position + length) > Length)
                throw new EndOfStreamException("Attempt to read past the length of the packet.");

            ReadOnlySpan<byte> span = _buffer.Span.Slice(Position, length);
            Position += length;
            return span;
        }

        public bool ReadBool()
        {
            return _buffer.Span[Position++] != 0;
        }

        public byte ReadByte()
        {
            return _buffer.Span[Position++];
        }

        public short ReadShort()
        {
            return BinaryPrimitives.ReadInt16BigEndian(SliceAndAdvance(2));
        }

        public int ReadInt()
        {
            return BinaryPrimitives.ReadInt32BigEndian(SliceAndAdvance(4));
        }

        public long ReadLong()
        {
            return BinaryPrimitives.ReadInt64BigEndian(SliceAndAdvance(8));
        }

        public string ReadString()
        {
            return Encoding.UTF8.GetString(SliceAndAdvance(ReadShort()));
        }

        public double ReadDouble()
        {
            return double.Parse(ReadString());
        }
    }*/
}
