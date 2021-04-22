using System;
using Xabbo.Messages;

namespace Xabbo.Core
{
    public interface IItemData : IComposable
    {
        StuffDataType Type { get; }

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
