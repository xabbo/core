using System;

namespace Xabbo.Core
{
    public enum FurniType
    {
        Floor,
        Wall
    }

    public static partial class XabboEnumExtensions
    {
        public static string ToShortString(this FurniType type)
        {
            switch (type)
            {
                case FurniType.Floor: return "S";
                case FurniType.Wall: return "I";
                default: throw new Exception($"Unknown furni type: {type}");
            }
        }
    }
}
