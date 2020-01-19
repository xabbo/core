using System;

namespace Xabbo.Core
{
    // this enum is a bit field
    public enum Gender
    {
        Male = 1,
        Female = 2,
        Unisex = 3
    }

    public static partial class EnumExtensions
    {
        public static string ToShortString(this Gender gender)
        {
            switch (gender)
            {
                case Gender.Male: return "M";
                case Gender.Female: return "F";
                case Gender.Unisex: return "U";
                default: throw new Exception($"Unknown gender '{gender}'.");
            }
        }
    }
}
