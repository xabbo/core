using System;

namespace Xabbo.Core
{
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
        public static string ToShortString(this ItemType type) => ((char)type).ToString();
    }
}
