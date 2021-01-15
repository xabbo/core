using System;

namespace Xabbo.Core.Messages
{
    public interface IPacket : IReadOnlyPacket
    {
        /// <summary>
        /// Gets or sets the message header of the packet.
        /// </summary>
        new Header Header { get; set; }

        /// <summary>
        /// Gets or sets the current position in the packet.
        /// </summary>
        new int Position { get; set; }

        /// <summary>
        /// Writes a boolean to the current position in the packet.
        /// </summary>
        void WriteBool(bool value);

        /// <summary>
        /// Writes a byte to the current position in the packet.
        /// </summary>
        void WriteByte(byte value);

        /// <summary>
        /// Writes a short to the current position in the packet.
        /// </summary>
        void WriteShort(short value);

        /// <summary>
        /// Writes an integer to the current position in the packet.
        /// </summary>
        void WriteInt(int value);

        /// <summary>
        /// Writes a double (as a string) to the current position in the packet.
        /// </summary>
        void WriteFloat(float value);

        /// <summary>
        /// Writes a long to the current position in the packet.
        /// </summary>
        /// <param name="value"></param>
        void WriteLong(long value);

        /// <summary>
        /// Writes a string to the current position in the packet.
        /// </summary>
        void WriteString(string value);

        /// <summary>
        /// Writes a floating point number as a string to the current position in the packet.
        /// </summary>
        void WriteFloatAsString(float value);

        /// <summary>
        /// Writes the specified bytes to the current position in the packet.
        /// </summary>
        void WriteBytes(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Writes the specified values to the current position in the packet.
        /// </summary>
        void WriteValues(params object[] values);

        /// <summary>
        /// Replaces a string at the current position in the packet.
        /// </summary>
        void ReplaceString(string newValue);

        /// <summary>
        /// Replaces a string at the specified position in the packet.
        /// </summary>
        void ReplaceString(string newValue, int position);

        /// <summary>
        /// Replaces the specified values at the current position in the packet.
        /// </summary>
        void ReplaceValues(params object[] newValues);

        /// <summary>
        /// Replaces the specified values at the specified position in the packet.
        /// </summary>
        void ReplaceValues(object[] newValues, int position);
    }
}
