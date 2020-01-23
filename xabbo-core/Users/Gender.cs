﻿using System;

namespace Xabbo.Core
{
    public enum Gender
    {
        Male = 0x01,
        Female = 0x02,
        Unisex = Male | Female
    }

    public static partial class XabboEnumExtensions
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
