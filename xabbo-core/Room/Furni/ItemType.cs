using System;

namespace Xabbo.Core
{
    public enum ItemType
    {
        Floor, // s
        Wall, // i
        Badge, // b
        Effect // e
    }

    public static partial class XabboEnumExtensions
    {
        public static string ToShortString(this ItemType type)
        {
            switch (type)
            {
                case ItemType.Floor: return "s";
                case ItemType.Wall: return "i";
                case ItemType.Badge: return "b";
                case ItemType.Effect: return "e";
                default: throw new Exception($"Unknown item type: {type}");
            }
        }
    }
}
