using System;

namespace Xabbo.Core;

/// <summary>
/// Represents an avatar's gender.
/// </summary>
public enum Gender
{
    None = -1,
    Female = 0,
    Male = 1,
    Unisex = 2
}

public static partial class H
{
    public static Gender ToGender(int value) => (Gender)value;
    public static Gender ToGender(string value) => value.ToLower() switch
    {
        "m" or "male" => Gender.Male,
        "f" or "female" => Gender.Female,
        "u" or "unisex" => Gender.Unisex,
        _ => throw new FormatException($"Unknown gender: '{value}'."),
    };
}

public static partial class XabboEnumExtensions
{
    /// <summary>
    /// Gets the uppercase single-character identifier of the specified gender.
    /// </summary>
    public static string ToClientString(this Gender gender)
    {
        return gender switch
        {
            Gender.Female => "F",
            Gender.Male => "M",
            Gender.Unisex => "U",
            _ => throw new Exception($"Unknown gender '{gender}'."),
        };
    }

    public static int ToClientValue(this Gender gender) => (int)gender;
}
