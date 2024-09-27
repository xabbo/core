using System;

namespace Xabbo.Core;

/// <summary>
/// Represents the type of an item.
/// </summary>
public enum ItemType
{
    None,
    Floor = 's',
    Wall = 'i',
    Badge = 'b',
    Effect = 'e',
    Bot = 'r'
}

public static partial class XabboEnumExtensions
{
    /// <summary>
    /// Gets the client value of the floor or wall item type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>
    /// One of the following:
    /// <list type="bullet">
    /// <item><see cref="ItemType.Wall"/>: <c>0</c></item>
    /// <item><see cref="ItemType.Floor"/>: <c>1</c></item>
    /// </list>
    /// </returns>
    /// <exception cref="Exception">
    /// The specified type is not <see cref="ItemType.Floor"/>
    /// or <see cref="ItemType.Wall"/>.</exception>
    public static short GetClientValue(this ItemType type) => type switch
    {
        ItemType.Wall => 0,
        ItemType.Floor => 1,
        _ => throw new Exception($"Unknown value for item type: {type}")
    };

    /// <summary>
    /// Gets the lowercase single-character identifier of the item type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>
    /// One of the following:
    /// <list type="bullet">
    /// <item><see cref="ItemType.Floor"/>: <c>"s"</c></item>
    /// <item><see cref="ItemType.Wall"/>: <c>"i"</c></item>
    /// <item><see cref="ItemType.Badge"/>: <c>"b"</c></item>
    /// <item><see cref="ItemType.Effect"/>: <c>"e"</c></item>
    /// <item><see cref="ItemType.Bot"/>: <c>"r"</c></item>
    /// </list>
    /// </returns>
    public static string GetClientIdentifier(this ItemType type) => ((char)type).ToString(); }
