using System;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Defines extra data attached to an item.
/// </summary>
public interface IItemData : IComposer
{
    /// <summary>
    /// Gets the type of this item data.
    /// </summary>
    ItemDataType Type { get; }

    /// <summary>
    /// Gets the flags of this item data.
    /// </summary>
    ItemDataFlags Flags { get; }

    /// <summary>
    /// Gets if this item data contains limited edition rare information.
    /// </summary>
    bool IsLimitedRare { get; }
    /// <summary>
    /// Gets the unique number in the limited edition rare series.
    /// </summary>
    int UniqueSerialNumber { get; }
    /// <summary>
    /// Gets the total number of limited edition rares in the series.
    /// </summary>
    int UniqueSeriesSize { get; }

    /// <summary>
    /// Gets the legacy string value of this item data.
    /// </summary>
    string Value { get; }

    /// <summary>
    /// Gets the state of the item data from the legacy string if it is of an integer format, otherwise returns -1.
    /// </summary>
    int State { get; }
}
