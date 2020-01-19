using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;
using System.Globalization;

namespace Xabbo.Core.Protocol
{
    public class Packet
    {
        private MemoryStream _ms;

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

        public short Header { get; set; }

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

        private byte[] buffer = new byte[4];

        public Packet()
        {
            _ms = new MemoryStream();
        }

        public Packet(short header)
            : this()
        {
            Header = header;
        }

        public Packet(byte[] buffer, int offset, int count, bool includesHeader = true)
            : this()
        {
            if (offset < 0 || offset >= buffer.Length)
                throw new ArgumentOutOfRangeException("offset");
            if (count < 0 || offset + count > buffer.Length || (includesHeader && count < 2))
                throw new ArgumentOutOfRangeException("length");

            if (includesHeader)
            {
                Header = (short)(
                    (buffer[offset] << 8) |
                    buffer[offset + 1]
                );
                _ms.Write(buffer, offset + 2, count - 2);
            }
            else
            {
                _ms.Write(buffer, offset, count);
            }

            Position = 0;
            Length = (int)_ms.Length;
        }

        public Packet(byte[] buffer, bool includesHeader = true)
            : this(buffer, 0, buffer.Length, includesHeader)
        { }

        public Packet(short header, params object[] values)
        {
            _ms = new MemoryStream();

            Header = header;
            WriteValues(values);
        }

        public bool CanReadByte() => Available >= 1;

        public bool CanReadBoolean()
        {
            if (!CanReadByte()) return false;
            byte b = ReadByte();
            Position -= 1;
            return b <= 1;
        }

        public bool CanReadShort() => Available >= 2;

        public bool CanReadInteger() => Available >= 4;

        public bool CanReadBytes(int count) => Available >= count;

        public bool CanReadString()
        {
            if (!CanReadShort()) return false;
            int len = ReadShort();
            Position -= 2;
            return CanReadBytes(2 + len);
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

        public bool ReadBoolean() => ReadByte() == 1;

        public bool ReadBoolean(int position)
        {
            Position = position;
            return ReadBoolean();
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

        public int ReadInteger()
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

        public int ReadInteger(int position)
        {
            Position = position;
            return ReadInteger(position);
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

            short len = ReadShort();
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

        public void WriteBoolean(bool value) => WriteByte((byte)(value ? 1 : 0));

        public void WriteBoolean(bool value, int position)
        {
            Position = position;
            WriteBoolean(value);
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

        public void WriteInteger(int value)
        {
            buffer[0] = (byte)((value >> 24) & 0xFF);
            buffer[1] = (byte)((value >> 16) & 0xFF);
            buffer[2] = (byte)((value >> 8) & 0xFF);
            buffer[3] = (byte)(value & 0xFF);
            Write(buffer, 0, 4);
        }

        public void WriteInteger(int value, int position)
        {
            Position = position;
            WriteInteger(value);
        }

        public void WriteDouble(double value)
        {
            WriteString(value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteDouble(double value, int position)
        {
            Position = position;
            WriteDouble(value);
        }

        public void WriteBytes(byte[] value)
        {
            Write(value, 0, value.Length);
        }

        public void WriteBytes(byte[] value, int position)
        {
            Position = position;
            WriteBytes(value);
        }

        public void WriteString(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
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
                    case bool x: WriteBoolean(x); break;
                    case short x: WriteShort(x); break;
                    case ushort x: WriteShort((short)x); break;
                    case int x: WriteInteger(x); break;
                    case byte[] x: WriteBytes(x); break;
                    case string x: WriteString(x); break;
                    case double x: WriteString(x.ToString()); break;
                    case IWritable x: x.Write(this); break;
                    case ICollection x:
                        {
                            WriteInteger(x.Count);
                            foreach (object o in x)
                                WriteValues(o);
                        }
                        break;
                    default: throw new Exception($"Invalid value type: {value.GetType().Name}");
                }
            }
        }

        #region - Replacement -
        public void ReplaceString(string value) => ReplaceString(value, Position);

        public void ReplaceString(string value, int position)
        {
            Position = position;
            if (!CanReadString())
                throw new InvalidOperationException($"Cannot read string at position {position}");

            int len = ReadShort();

            if (len == Encoding.UTF8.GetByteCount(value))
            {
                Position = position;
                WriteString(value);
                return;
            }

            Position += len;
            byte[] temp = ReadBytes(Length - Position);
            Position = position;
            WriteString(value);

            int finalPosition = Position;
            WriteBytes(temp);
            Length = Position;
            Position = finalPosition;
        }

        public void ReplaceValues(params object[] values)
        {
            foreach (object value in values)
            {
                switch (value)
                {
                    case byte x: WriteByte(x); break;
                    case bool x: WriteBoolean(x); break;
                    case short x: WriteShort(x); break;
                    case ushort x: WriteShort((short)x); break;
                    case int x: WriteInteger(x); break;
                    case byte[] x: WriteBytes(x); break;
                    case string x: ReplaceString(x); break;
                    case Type t:
                        {
                            if (t == typeof(byte)) ReadByte();
                            else if (t == typeof(bool)) ReadBoolean();
                            else if (t == typeof(short)) ReadShort();
                            else if (t == typeof(ushort)) ReadShort();
                            else if (t == typeof(int)) ReadInteger();
                            else if (t == typeof(byte[])) throw new NotSupportedException();
                            else if (t == typeof(string)) ReadString();
                            else throw new Exception($"Invalid type: {t.FullName}");
                        }
                        break;
                    default: throw new Exception($"Invalid value type: {value.GetType().Name}");
                }
            }
        }

        public void ReplaceValues(object[] values, int position)
        {
            Position = position;
            ReplaceValues(values);
        }
        #endregion

        public Packet Clone()
        {
            byte[] bytes = ToBytes();
            return new Packet(bytes, 4, bytes.Length - 4);
        }
    }
}
