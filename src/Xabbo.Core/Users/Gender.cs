using System;

namespace Xabbo.Core;

[Flags]
public enum Gender
{
    None = 0,
    Male = 1,
    Female = 2,
    Unisex = Male | Female
}

public static partial class XabboEnumExtensions
{
    public static string ToShortString(this Gender gender)
    {
        return gender switch
        {
            Gender.Male => "M",
            Gender.Female => "F",
            Gender.Unisex => "U",
            _ => throw new Exception($"Unknown gender '{gender}'."),
        };
    }

    public static int GetValue(this Gender gender)
    {
        return gender switch
        {
            Gender.Male => 1,
            Gender.Female => 0,
            Gender.Unisex => 2,
            _ => -1,
        };
    }
}
