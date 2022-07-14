using System;

namespace Xabbo.Core;

public enum ItemType
{
    Floor = 's',
    Wall = 'i',
    Badge = 'b',
    Effect = 'e',
    Bot = 'r'
}

public static partial class XabboEnumExtensions
{
    public static short GetValue(this ItemType type) => type switch
    {
        ItemType.Floor => 1,
        ItemType.Wall => 0,
        _ => throw new Exception($"Unknown value for item type: {type}")
    };

    public static string ToShortString(this ItemType type) => ((char)type).ToString();
}
