using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Globalization;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Protocol
{
    public class Packet : IPacket
    {
        private class ReadOnlyWrapper : IReadOnlyPacket
        {
            private readonly Packet packet;

            public Header Header => packet.Header;

            public int Position
            {
                get => packet.Position;
                set => packet.Position = value;
            }

            public int Length => packet.Length;

            public int Available => packet.Available;

            public ReadOnlyMemory<byte> GetBuffer() => packet.GetData();

            public ReadOnlyWrapper(Packet packet)
            {
                this.packet = packet;
            }

            public bool CanReadBool() => packet.CanReadBool();

            public bool CanReadByte() => packet.CanReadByte();

            public bool CanReadDouble() => packet.CanReadDouble();

            public bool CanReadInt() => packet.CanReadInt();

            public bool CanReadShort() => packet.CanReadShort();

            public bool CanReadString() => packet.CanReadString();

            public byte[] GetData() => packet.GetData();

            public bool ReadBool() => packet.ReadBool();

            public bool ReadBool(int position) => packet.ReadBool(position);

            public byte ReadByte() => packet.ReadByte();

            public byte ReadByte(int position) => packet.ReadByte(position);

            public byte[] ReadBytes(int count) => packet.ReadBytes(count);

            public double ReadDouble() => packet.ReadDouble();

            public double ReadDouble(int position) => packet.ReadDouble(position);

            public int ReadInt() => packet.ReadInt();

            public int ReadInt(int position) => packet.ReadInt(position);

            public short ReadShort() => packet.ReadShort();

            public short ReadShort(int position) => packet.ReadShort(position);

            public string ReadString() => packet.ReadString();

            public string ReadString(int position) => packet.ReadString(position);

            public byte[] ToBytes() => packet.ToBytes();
        }

        public static readonly Type
            Byte = typeof(byte),
            Bool = typeof(bool),
            Short = typeof(short),
            Int = typeof(int),
            Long = typeof(long),
            ByteArray = typeof(byte[]),
            String = typeof(string);

        private readonly MemoryStream _ms;

        public ReadOnlyMemory<byte> GetBuffer()
        {
            return new ReadOnlyMemory<byte>(_ms.GetBuffer(), 0, Length);
        }

        /// <summary>
        /// Gets the data in the packet including the length and message ID headers.
        /// </summary>
        public byte[] ToBytes()
        {
            byte[] data = new byte[6 + Length];

            int len = 2 + Length;
            data[0] = (byte)((len >> 24) & 0xFF);
            data[1] = (byte)((len >> 16) & 0xFF);
            data[2] = (byte)((len >> 8) & 0xFF);
            data[3] = (byte)(len & 0xFF);

            data[4] = (byte)((Header >> 8) & 0xFF);
            data[5] = (byte)(Header & 0xFF);

            if (Length > 0)
            {
                int position = Position;
                Position = 0;
                _ms.Read(data, 6, Length);
                Position = position;
            }

            return data;
        }

        /// <summary>
        /// Gets the data in the packet excluding the length and message ID header.
        /// </summary>
        public byte[] GetData()
        {
            byte[] data = new byte[Length];
            int pos = Position;
            Position = 0;
            _ms.Read(data, 0, Length);
            Position = pos;
            return data;
        }

        public Header Header { get; set; } = Header.Unknown;

        public int Position
        {
            get => (int)_ms.Position;
            set
            {
                if (value < 0 || value > Length) throw new IndexOutOfRangeException();
                _ms.Position = value;
            }
        }

        public int Length { get; private set; }

        public int Available => Length - Position;

        private byte[] buffer = new byte[8];

        public Packet()
        {
            _ms = new MemoryStream();
        }

        public Packet(Header header)
            : this()
        {
            Header = header;
        }

        public Packet(byte[] data, int offset, int count, bool includesHeader = true, Destination destination = Destination.Unknown)
            : this()
        {
            if (offset < 0 || offset > data.Length)
                throw new ArgumentOutOfRangeException("offset");
            if (count < 0 || (offset + count) > data.Length || (includesHeader && count < 2))
                throw new ArgumentOutOfRangeException("count");

            if (includesHeader)
            {
                Header = new Header(
                    destination,
                    (short)(
                        (data[offset] << 8) |
                        data[offset + 1]
                    )
                );
                _ms.Write(data, offset + 2, count - 2);
            }
            else
            {
                _ms.Write(data, offset, count);
            }

            Position = 0;
            Length = (int)_ms.Length;
        }

        public Packet(byte[] data, bool includesHeader = true)
            : this(data, 0, data.Length, includesHeader)
        { }

        public Packet(Header header, params object[] values)
        {
            _ms = new MemoryStream();

            Header = header;
            WriteValues(values);
        }

        public Packet(IReadOnlyPacket packet)
            : this(packet.Header)
        {
            byte[] data = packet.GetData();
            _ms.Write(data, 0, data.Length);
            _ms.Position = 0;
            Length = (int)_ms.Length;
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
            if (!CanReadShort()) return false;
            int len = ReadShort();
            Position -= 2;
            return Available >= (2 + len);
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

            _ms.Read(buffer, 0, 1);
            return buffer[0];
        }

        public byte ReadByte(int position)
        {
            if (position + 1 > Length)
                throw new EndOfStreamException();

            Position = position;
            return ReadByte();
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

            _ms.Read(buffer, 0, 2);
            return (short)(
                (buffer[0] << 8) |
                buffer[1]
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

            _ms.Read(buffer, 0, 4);
            return
                (buffer[0] << 24) |
                (buffer[1] << 16) |
                (buffer[2] << 8) |
                buffer[3];
        }

        public int ReadInt(int position)
        {
            Position = position;
            return ReadInt();
        }

        public double ReadDouble()
        {
            return double.Parse(ReadString(), CultureInfo.InvariantCulture);
        }

        public double ReadDouble(int position)
        {
            Position = position;
            return ReadDouble();
        }

        public byte[] ReadBytes(int count)
        {
            if (Available < count)
                throw new EndOfStreamException();

            byte[] bytes = new byte[count];
            _ms.Read(bytes, 0, count);
            return bytes;
        }

        public byte[] ReadBytes(int count, int position)
        {
            Position = position;
            return ReadBytes(count);
        }

        public string ReadString()
        {
            if (!CanReadString())
                throw new EndOfStreamException();

            int len = (ushort)ReadShort();
            byte[] bytes = ReadBytes(len);

            return Encoding.UTF8.GetString(bytes);
        }

        public string ReadString(int position)
        {
            Position = position;
            return ReadString();
        }

        private void Write(byte[] buffer, int offset, int count)
        {
            _ms.Write(buffer, offset, count);
            if (Length < Position) Length = Position;
        }

        public void WriteByte(byte value)
        {
            buffer[0] = value;
            Write(buffer, 0, 1);
        }

        public void WriteByte(byte value, int position)
        {
            Position = position;
            WriteByte(value);
        }

        public void WriteBool(bool value) => WriteByte((byte)(value ? 1 : 0));

        public void WriteBool(bool value, int position)
        {
            Position = position;
            WriteBool(value);
        }

        public void WriteShort(short value)
        {
            buffer[0] = (byte)((value >> 8) & 0xFF);
            buffer[1] = (byte)(value & 0xFF);
            Write(buffer, 0, 2);
        }

        public void WriteShort(short value, int position)
        {
            Position = position;
            WriteShort(value);
        }

        public void WriteInt(int value)
        {
            buffer[0] = (byte)((value >> 24) & 0xFF);
            buffer[1] = (byte)((value >> 16) & 0xFF);
            buffer[2] = (byte)((value >> 8) & 0xFF);
            buffer[3] = (byte)(value & 0xFF);
            Write(buffer, 0, 4);
        }

        public void WriteInt(int value, int position)
        {
            Position = position;
            WriteInt(value);
        }

        public void WriteLong(long value)
        {
            buffer[0] = (byte)((value >> 56) & 0xFF);
            buffer[1] = (byte)((value >> 48) & 0xFF);
            buffer[2] = (byte)((value >> 40) & 0xFF);
            buffer[3] = (byte)((value >> 32) & 0xFF);
            buffer[4] = (byte)((value >> 24) & 0xFF);
            buffer[5] = (byte)((value >> 16) & 0xFF);
            buffer[6] = (byte)((value >> 8) & 0xFF);
            buffer[7] = (byte)(value >> 0 & 0xFF);
            Write(buffer, 0, 8);
        }

        public void WriteLong(long value, int position)
        {
            Position = position;
            WriteLong(value);
        }

        public void WriteDouble(double value)
        {
            WriteString(value.ToString("0.0##############", CultureInfo.InvariantCulture));
        }

        public void WriteDouble(double value, int position)
        {
            Position = position;
            WriteDouble(value);
        }

        public void WriteBytes(byte[] bytes)
        {
            Write(bytes, 0, bytes.Length);
        }

        public void WriteBytes(byte[] bytes, int position)
        {
            Position = position;
            WriteBytes(bytes);
        }

        public void WriteString(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
            WriteShort((short)bytes.Length);
            WriteBytes(bytes);
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
                    case double x: WriteDouble(x); break;
                    case IPacketData x: x.Write(this); break;
                    case ICollection x:
                        {
                            WriteInt(x.Count);
                            foreach (object o in x)
                                WriteValues(o);
                        }
                        break;
                    case IEnumerable x:
                        {
                            int count = 0, startPosition = Position;
                            WriteInt(-1);
                            foreach (object o in x)
                            {
                                WriteValues(o);
                                count++;
                            }
                            int endPosition = Position;
                            WriteInt(count, startPosition);
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
            Position = position;
            if (!CanReadString())
                throw new InvalidOperationException($"Cannot read string at position {position}");

            int len = ReadShort();

            byte[] stringBytes = Encoding.UTF8.GetBytes(newValue);

            if (len == stringBytes.Length)
            {
                WriteBytes(stringBytes);
                return;
            }

            Position += len;
            byte[] temp = ReadBytes(Length - Position);
            Position = position;
            WriteShort((short)stringBytes.Length);
            WriteBytes(stringBytes);

            int finalPosition = Position;
            WriteBytes(temp);
            Length = Position;
            Position = finalPosition;
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

        public IReadOnlyPacket AsReadOnly() => new ReadOnlyWrapper(this);
        #endregion
    }
}
