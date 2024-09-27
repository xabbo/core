namespace Xabbo.Core;

/// <summary>
/// Represents extra data attached to an item.
/// </summary>
public interface IItemData
{
    /// <summary>
    /// Gets the type of the item data.
    /// </summary>
    ItemDataType Type { get; }

    /// <summary>
    /// Gets the flags of the item data.
    /// </summary>
    ItemDataFlags Flags { get; }

    /// <summary>
    /// Whether the item data has limited edition rare (LTD) information.
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
    /// Gets the legacy string value of the item data.
    /// </summary>
    string Value { get; }

    /// <summary>
    /// Gets the state of the item data from the legacy string if it is of an integer format, otherwise returns -1.
    /// </summary>
    int State { get; }
}
