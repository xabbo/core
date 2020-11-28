using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public interface IItemData : IPacketData
    {
        ItemDataType Type { get; }

        /// <summary>
        /// Gets the flags of the item data.
        /// </summary>
        ItemDataFlags Flags { get; }

        int LimitedNumber { get; }
        int LimitedTotal { get; }

        string Value { get; }

        /// <summary>
        /// Gets the state of the item data from the legacy string if it is of an integer format, otherwise returns -1.
        /// </summary>
        int State { get; }
    }
}
