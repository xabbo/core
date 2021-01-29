using System;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Protocol
{
    public interface IReadOnlyPacket
    {
        /// <summary>
        /// Gets the internal data buffer of the packet as <see cref="ReadOnlyMemory{T}"/>.
        /// </summary>
        ReadOnlyMemory<byte> GetBuffer();

        /// <summary>
        /// Copies the data of the packet into a <see cref="Span{T}"/>.
        /// </summary>
        void CopyTo(Span<byte> destination);

        /// <summary>
        /// Gets the message header of the packet.
        /// </summary>
        Header Header { get; }

        /// <summary>
        /// Gets or sets the current position in the packet.
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
        /// Returns true only if the next available byte is either 0 or 1.
        /// </summary>
        bool CanReadBool();

        /// <summary>
        /// Gets if a string can be read from the current position in the packet.
        /// </summary>
        bool CanReadString();

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
        /// Reads a 32-bit floating point number from the current position in the packet.
        /// </summary>
        float ReadFloat();

        /// <summary>
        /// Reads a 32-bit floating point number from the specified position in the packet.
        /// </summary>
        float ReadFloat(int position);

        /// <summary>
        /// Reads a long from the current position in the packet.
        /// </summary>
        /// <returns></returns>
        long ReadLong();

        /// <summary>
        /// Reads a long from the specified positions in the packet.
        /// </summary>
        long ReadLong(int position);

        /// <summary>
        /// Reads a string from the current position in the packet.
        /// </summary>
        string ReadString();

        /// <summary>
        /// Reads a string from the specified position in the packet.
        /// </summary>
        string ReadString(int position);

        /// <summary>
        /// Reads a string from the current position in the packet and parses it into a floating point number.
        /// </summary>
        float ReadFloatAsString();

        /// <summary>
        /// Reads a string from the specified position in the packet and parses it into a floating point number.
        /// </summary>
        float ReadFloatAsString(int position);

        /// <summary>
        /// Copies from the current position in the packet into the <see cref="Span{T}"/>.
        /// </summary>
        void ReadBytes(Span<byte> buffer);

        /// <summary>
        /// Copies from the specified position in the packet into the <see cref="Span{T}"/>
        /// </summary>
        void ReadBytes(Span<byte> buffer, int position);
    }
}
