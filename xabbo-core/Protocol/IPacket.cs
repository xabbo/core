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
        /// Gets or sets the current position of the packet.
        /// </summary>
        new int Position { get; set; }

        /// <summary>
        /// Writes the specified boolean to the current position in the packet.
        /// </summary>
        void WriteBool(bool value);

        /// <summary>
        /// Writes the specified byte to the current position in the packet.
        /// </summary>
        void WriteByte(byte value);

        /// <summary>
        /// Writes the specified short to the current position in the packet.
        /// </summary>
        void WriteShort(short value);

        /// <summary>
        /// Writes the specified integer to the current position in the packet.
        /// </summary>
        void WriteInt(int value);

        /// <summary>
        /// Writes the specified string to the current position in the packet.
        /// </summary>
        void WriteString(string value);

        /// <summary>
        /// Writes the specified double to the current position in the packet.
        /// </summary>
        void WriteDouble(double value);

        /// <summary>
        /// Writes the specified bytes to the current position in the packet.
        /// </summary>
        void WriteBytes(byte[] bytes);

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
        /// Replaces values at the current position in the packet.
        /// </summary>
        void ReplaceValues(params object[] newValues);

        /// <summary>
        /// Replaces values at the specified position in the packet.
        /// </summary>
        void ReplaceValues(object[] newValues, int position);
    }
}
