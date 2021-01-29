using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Globalization;
using System.Buffers.Binary;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Protocol
{
    public class Packet : IPacket
    {
        public static readonly Type
            Byte = typeof(byte),
            Bool = typeof(bool),
            Short = typeof(short),
            Int = typeof(int),
            Float = typeof(float),
            Long = typeof(long),
            String = typeof(string),
            ByteArray = typeof(byte[]);

        public ReadOnlyMemory<byte> GetBuffer() => _buffer[0..Length];

        public void CopyTo(Span<byte> destination) => _buffer.Span[0..Length].CopyTo(destination);

        public Header Header { get; set; } = Header.Unknown;

        private int _position;
        public int Position
        {
            get => _position;
            set
            {
                if (value < 0 || value > Length)
                    throw new IndexOutOfRangeException();
                _position = value;
            }
        }

        public int Length { get; private set; }

        public int Available => Length - Position;

        public Packet()
        {
            Header = Header.Unknown;
            _buffer = new byte[8];
        }

        private Memory<byte> _buffer;

        public Packet(ReadOnlySpan<byte> buffer)
        {
            Header = Header.Unknown;
            _buffer = new Memory<byte>(buffer.ToArray());
        }

        private void Grow(int length) => GrowToSize(Position + length);

        private void GrowToSize(int minSize)
        {
            if (_buffer.Length < minSize)
            {
                int size = _buffer.Length;
                while (size < minSize)
                    size <<= 1;

                Memory<byte> oldMemory = _buffer;
                _buffer = new Memory<byte>(new byte[size]);
                oldMemory.CopyTo(_buffer);
            }

            if (Length < minSize)
                Length = minSize;
        }

        public Packet(Header header)
        {
            Header = header;
            _buffer = new byte[32];
        }

        public Packet(Header header, ReadOnlySpan<byte> span)
        {
            Header = header;
            _buffer = span.ToArray();

            Position = 0;
            Length = span.Length;
        }

        public Packet(Header header, params object[] values)
        {
            Header = header;
            _buffer = new byte[32];

            WriteValues(values);

            Position = 0;
        }

        public Packet(IReadOnlyPacket packet)
        {
            Header = packet.Header;
            _buffer = new byte[packet.Length];
            packet.CopyTo(_buffer.Span);

            Position = 0;
            Length = packet.Length;
        }

        public static Packet Compose(Header header, params object[] values)
        {
            var packet = new Packet(header);
            packet.WriteValues(values);
            return packet;
        }

        public bool CanReadByte() => Available >= 1;

        public bool CanReadBool()
        {
            if (!CanReadByte()) return false;
            byte b = ReadByte();
            Position -= 1;
            return b <= 1;
        }

        public bool CanReadShort() => Available >= 2;

        public bool CanReadInt() => Available >= 4;

        public bool CanReadString()
        {
            if (Available < 2) return false;

            return Available >= 2 + BinaryPrimitives.ReadUInt16BigEndian(_buffer.Span[Position..]);
        }

        public bool CanReadDouble()
        {
            if (!CanReadString()) return false;

            int pos = Position;
            bool success = double.TryParse(ReadString(), NumberStyles.Float, CultureInfo.InvariantCulture, out _);
            Position = pos;

            return success;
        }

        public byte ReadByte()
        {
            if (Available < 1)
                throw new EndOfStreamException();

            Position++;
            return _buffer.Span[Position - 1];
        }

        public byte ReadByte(int position)
        {
            if (position + 1 > Length)
                throw new EndOfStreamException();

            Position = position;
            return ReadByte();
        }

        public void ReadBytes(Span<byte> buffer)
        {
            if (Available < buffer.Length)
                throw new EndOfStreamException();

            Position += buffer.Length;
            _buffer.Span[(Position - buffer.Length)..Position].CopyTo(buffer);
        }

        public void ReadBytes(Span<byte> buffer, int position)
        {
            Position = position;
            ReadBytes(buffer);
        }

        public bool ReadBool() => ReadByte() != 0;

        public bool ReadBool(int position)
        {
            Position = position;
            return ReadBool();
        }

        public short ReadShort()
        {
            if (Available < 2)
                throw new EndOfStreamException();

            Position += 2;
            return BinaryPrimitives.ReadInt16BigEndian(
                _buffer.Span[(Position - 2)..]
            );
        }

        public short ReadShort(int position)
        {
            Position = position;
            return ReadShort();
        }

        public int ReadInt()
        {
            if (Available < 4)
                throw new EndOfStreamException();

            Position += 4;
            return BinaryPrimitives.ReadInt32BigEndian(
                _buffer.Span[(Position - 4)..]
            );
        }

        public int ReadInt(int position)
        {
            Position = position;
            return ReadInt();
        }

        public float ReadFloat()
        {
            if (Available < 4)
                throw new EndOfStreamException();

            Position += 4;
            return BinaryPrimitives.ReadSingleBigEndian(
                _buffer.Span[(Position - 4)..]
            );
        }

        public float ReadFloat(int position)
        {
            Position = position;
            return ReadFloat();
        }

        public long ReadLong()
        {
            if (Available < 8)
                throw new EndOfStreamException();

            Position += 8;
            return BinaryPrimitives.ReadInt64BigEndian(
                _buffer.Span[(Position - 8)..]
            );
        }

        public long ReadLong(int position)
        {
            Position = position;
            return ReadLong();
        }

        public string ReadString()
        {
            if (!CanReadString())
                throw new EndOfStreamException();

            int len = (ushort)ReadShort();
            Position += len;

            return Encoding.UTF8.GetString(_buffer.Span[(Position - len)..Position]);
        }

        public string ReadString(int position)
        {
            Position = position;
            return ReadString();
        }

        public float ReadFloatAsString()
        {
            return float.Parse(ReadString(), CultureInfo.InvariantCulture);
        }

        public float ReadFloatAsString(int position)
        {
            Position = position;
            return ReadFloatAsString();
        }

        public void WriteByte(byte value)
        {
            Grow(1);
            _buffer.Span[Position++] = value;
        }

        public void WriteByte(byte value, int position)
        {
            Position = position;
            WriteByte(value);
        }

        public void WriteBytes(ReadOnlySpan<byte> bytes)
        {
            Grow(bytes.Length);
            bytes.CopyTo(_buffer.Span[Position..]);
            Position += bytes.Length;
        }

        public void WriteBytes(ReadOnlySpan<byte> bytes, int position)
        {
            Position = position;
            WriteBytes(bytes);
        }

        public void WriteBool(bool value) => WriteByte((byte)(value ? 1 : 0));

        public void WriteBool(bool value, int position)
        {
            Position = position;
            WriteBool(value);
        }

        public void WriteShort(short value)
        {
            Grow(2);
            BinaryPrimitives.WriteInt16BigEndian(_buffer.Span[Position..], value);
            Position += 2;
        }

        public void WriteShort(short value, int position)
        {
            Position = position;
            WriteShort(value);
        }

        public void WriteInt(int value)
        {
            Grow(4);
            BinaryPrimitives.WriteInt32BigEndian(_buffer.Span[Position..], value);
            Position += 4;
        }

        public void WriteInt(int value, int position)
        {
            Position = position;
            WriteInt(value);
        }

        public void WriteLong(long value)
        {
            Grow(8);
            BinaryPrimitives.WriteInt64BigEndian(_buffer.Span[Position..], value);
            Position += 8;
        }

        public void WriteLong(long value, int position)
        {
            Position = position;
            WriteLong(value);
        }

        public void WriteFloat(float value)
        {
            Grow(4);
            BinaryPrimitives.WriteSingleBigEndian(_buffer.Span[Position..], value);
            Position += 4;
        }

        public void WriteFloat(float value, int position)
        {
            Position = position;
            WriteFloat(value);
        }

        public void WriteFloatAsString(float value)
        {
            WriteString(value.ToString("0.0##############", CultureInfo.InvariantCulture));
        }

        public void WriteFloatAsString(float value, int position)
        {
            Position = position;
            WriteFloatAsString(value);
        }



        public void WriteString(string value)
        {
            int len = Encoding.UTF8.GetByteCount(value);
            WriteShort((short)len);

            Grow(len);
            Encoding.UTF8.GetBytes(value, _buffer.Span[Position..]);
            Position += len;
        }

        public void WriteString(string value, int position)
        {
            Position = position;
            WriteString(value, position);
        }

        public void WriteValues(params object[] values)
        {
            foreach (object value in values)
            {
                switch (value)
                {
                    case byte x: WriteByte(x); break;
                    case bool x: WriteBool(x); break;
                    case short x: WriteShort(x); break;
                    case ushort x: WriteShort((short)x); break;
                    case int x: WriteInt(x); break;
                    case long x: WriteLong(x); break;
                    case byte[] x:
                        WriteInt(x.Length);
                        WriteBytes(x);
                        break;
                    case string x: WriteString(x); break;
                    case float x: WriteFloat(x); break;
                    case IPacketData x: x.Write(this); break;
                    case ICollection x:
                        {
                            WriteShort((short)x.Count);
                            foreach (object o in x)
                                WriteValues(o);
                        }
                        break;
                    case IEnumerable x:
                        {
                            int count = 0, startPosition = Position;
                            WriteShort(-1);
                            foreach (object o in x)
                            {
                                WriteValues(o);
                                count++;
                            }
                            int endPosition = Position;
                            WriteShort((short)count, startPosition);
                            Position = endPosition;
                        }
                        break;
                    default:
                        if (value == null)
                            throw new Exception("Null value");
                        else
                            throw new Exception($"Invalid value type: {value.GetType().Name}");
                }
            }
        }

        #region - Replacement -
        public void ReplaceString(string newValue) => ReplaceString(newValue, Position);

        public void ReplaceString(string newValue, int position)
        {
            int previousLen = BinaryPrimitives.ReadInt16BigEndian(_buffer.Span[position..]);
            if (Length < position + 2 + previousLen)
                throw new InvalidOperationException($"Cannot replace string at position {position}");

            int newLen = Encoding.UTF8.GetByteCount(newValue);

            int diff = newLen - previousLen;

            if (diff == 0)
            {
                Encoding.UTF8.GetBytes(newValue, _buffer.Span[Position..]);
                Position = position + 2 + newLen;
                return;
            }
            else if (diff > 0)
            {
                GrowToSize(Length + (newLen - previousLen));
            }
            else if (diff < 0)
            {
                Length -= newLen - previousLen;
            }

            byte[] tail = _buffer.Span[(position + 2 + previousLen)..].ToArray();

            BinaryPrimitives.WriteInt16BigEndian(_buffer.Span[position..], (short)newLen);
            Encoding.UTF8.GetBytes(newValue, _buffer.Span[(position + 2)..]);
            tail.CopyTo(_buffer[(position + 2 + newLen)..]);

            Position = position + 2 + newLen;
        }

        public void ReplaceValues(params object[] newValues)
        {
            foreach (object value in newValues)
            {
                switch (value)
                {
                    case byte x: WriteByte(x); break;
                    case bool x: WriteBool(x); break;
                    case short x: WriteShort(x); break;
                    case ushort x: WriteShort((short)x); break;
                    case int x: WriteInt(x); break;
                    case long x: WriteLong(x); break;
                    case byte[] x: WriteBytes(x); break;
                    case string x: ReplaceString(x); break;
                    case Type t:
                        {
                            if (t == Byte) ReadByte();
                            else if (t == Bool) ReadBool();
                            else if (t == Short) ReadShort();
                            else if (t == Int) ReadInt();
                            else if (t == ByteArray) throw new NotSupportedException();
                            else if (t == String) ReadString();
                            else throw new Exception($"Invalid type specified: {t.FullName}");
                        }
                        break;
                    default: throw new Exception($"Value is of invalid type: {value.GetType().Name}");
                }
            }
        }

        public void ReplaceValues(object[] newValues, int position)
        {
            Position = position;
            ReplaceValues(newValues);
        }
        #endregion
    }
}
