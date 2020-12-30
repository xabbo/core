using System;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Protocol
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
        /// Writes a boolean at the current position in the packet.
        /// </summary>
        void WriteBool(bool value);

        /// <summary>
        /// Writes a byte at the current position in the packet.
        /// </summary>
        void WriteByte(byte value);

        /// <summary>
        /// Writes a short at the current position in the packet.
        /// </summary>
        void WriteShort(short value);

        /// <summary>
        /// Writes an integer at the current position in the packet.
        /// </summary>
        void WriteInt(int value);

        /// <summary>
        /// Writes a long at the current position in the packet.
        /// </summary>
        /// <param name="value"></param>
        void WriteLong(long value);

        /// <summary>
        /// Writes a string at the current position in the packet.
        /// </summary>
        void WriteString(string value);

        /// <summary>
        /// Writes a double (as a string) at the current position in the packet.
        /// </summary>
        void WriteDouble(double value);

        /// <summary>
        /// Writes an array of bytes at the current position in the packet.
        /// </summary>
        void WriteBytes(byte[] bytes);

        /// <summary>
        /// Writes values at the current position in the packet.
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
        /// Replaces values at the current position in the packet.
        /// </summary>
        void ReplaceValues(params object[] newValues);

        /// <summary>
        /// Replaces values at the specified position in the packet.
        /// </summary>
        void ReplaceValues(object[] newValues, int position);
    }
}
