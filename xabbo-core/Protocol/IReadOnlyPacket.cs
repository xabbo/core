using System;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Protocol
{
    public interface IReadOnlyPacket
    {
        /// <summary>
        /// Gets a byte array of the packet, including the 4-byte length and 2-byte message ID headers.
        /// </summary>
        byte[] ToBytes();

        /// <summary>
        /// Gets a byte array of the data in the packet.
        /// </summary>
        byte[] GetData();

        ReadOnlyMemory<byte> GetBuffer();

        /// <summary>
        /// Gets the message header of the packet.
        /// </summary>
        Header Header { get; }

        /// <summary>
        /// Gets or sets the current position of the packet.
        /// </summary>
        int Position { get; set; }

        /// <summary>
        /// Gets the length of the data in the packet.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets the number of available bytes left in the packet.
        /// </summary>
        int Available { get; }

        /// <summary>
        /// Gets if a boolean can be read from the current position in the packet.
        /// </summary>
        bool CanReadBool();

        /// <summary>
        /// Gets if a byte can be read from the current position in the packet.
        /// </summary>
        bool CanReadByte();

        /// <summary>
        /// Gets if a short can be read from the current position in the packet.
        /// </summary>
        bool CanReadShort();

        /// <summary>
        /// Gets if an integer can be read from the current position in the packet.
        /// </summary>
        bool CanReadInt();

        /// <summary>
        /// Gets if a string can be read from the current position in the packet.
        /// </summary>
        bool CanReadString();

        /// <summary>
        /// Gets if a double can be read from the current position in the packet.
        /// </summary>
        bool CanReadDouble();

        /// <summary>
        /// Reads a boolean from the current position in the packet.
        /// </summary>
        bool ReadBool();
        
        /// <summary>
        /// Reads a boolean from the specified position in the packet.
        /// </summary>
        bool ReadBool(int position);

        /// <summary>
        /// Reads a byte from the current position in the packet.
        /// </summary>
        byte ReadByte();

        /// <summary>
        /// Reads a byte from the specified position in the packet.
        /// </summary>
        byte ReadByte(int position);

        /// <summary>
        /// Reads a short from the current position in the packet.
        /// </summary>
        short ReadShort();

        /// <summary>
        /// Reads a  from the specified position in the packet.
        /// </summary>
        short ReadShort(int position);

        /// <summary>
        /// Reads an integer from the current position in the packet.
        /// </summary>
        int ReadInt();

        /// <summary>
        /// Reads an integer from the specified position in the packet.
        /// </summary>
        int ReadInt(int position);

        /// <summary>
        /// Reads a string from the current position in the packet.
        /// </summary>
        string ReadString();

        /// <summary>
        /// Reads a string from the specified position in the packet.
        /// </summary>
        string ReadString(int position);

        /// <summary>
        /// Reads a double from the current position in the packet.
        /// </summary>
        double ReadDouble();

        /// <summary>
        /// Reads a double from the specified position in the packet.
        /// </summary>
        double ReadDouble(int position);

        /// <summary>
        /// Reads the specified number of bytes from the current position in the packet.
        /// </summary>
        byte[] ReadBytes(int count);
    }
}
