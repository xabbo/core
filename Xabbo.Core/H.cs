using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Xabbo.Core.Serialization;

namespace Xabbo.Core;

public static class H
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters =
        {
            new DateTimeConverter()
        }
    };

    private static readonly Regex regexAvatarValidator
           = new Regex(@"^[a-zA-Z0-9_\-=?!@:.,]{3,15}$", RegexOptions.Compiled);

    public static string UserAgent = XabboConst.DEFAULT_USER_AGENT;

    public static bool IsValidAvatarName(string avatarName) => regexAvatarValidator.IsMatch(avatarName);

    public static Gender ToGender(int gender)
    {
        return gender switch
        {
            0 => Gender.Female,
            1 => Gender.Male,
            _ => Gender.Unisex,
        };
    }

    public static Gender ToGender(string gender)
    {
        return (gender.ToLower()) switch
        {
            "m" or "male" => Gender.Male,
            "f" or "female" => Gender.Female,
            "u" or "unisex" => Gender.Unisex,
            _ => throw new FormatException($"Unknown gender: {gender}"),
        };
    }

    public static ItemType ToItemType(string s)
    {
        if (s.Length != 1)
            throw new Exception($"Invalid item type: {s}");

        ItemType type = (ItemType)s.ToLower()[0];
        if (!Enum.IsDefined(typeof(ItemType), type))
            throw new Exception($"Unknown item type: {s}");

        return type;
    }

    public static ItemType ToItemType(int value)
    {
        return value switch
        {
            0 => ItemType.Wall,
            1 => ItemType.Floor,
            _ => throw new Exception($"Unknown item type {value}")
        };
    }

    public static ItemType ToItemType(char value)
    {
        return value switch
        {
            's' => ItemType.Floor,
            'i' => ItemType.Wall,
            'b' => ItemType.Badge,
            'e' => ItemType.Effect,
            'r' => ItemType.Bot,
            _ => throw new Exception($"Unknown item type '{value}'")
        };
    }

    #region - Figure -
    public static bool TryGetFigurePartType(string partTypeString, out FigurePartType partType)
    {
        switch (partTypeString.ToLower())
        {
            case "hr": partType = FigurePartType.Hair; break;
            case "hd": partType = FigurePartType.Head; break;
            case "ch": partType = FigurePartType.Chest; break;
            case "lg": partType = FigurePartType.Legs; break;
            case "sh": partType = FigurePartType.Shoes; break;
            case "ha": partType = FigurePartType.Hat; break;
            case "he": partType = FigurePartType.HeadAccessory; break;
            case "ea": partType = FigurePartType.EyeAccessory; break;
            case "fa": partType = FigurePartType.FaceAccessory; break;
            case "ca": partType = FigurePartType.ChestAccessory; break;
            case "wa": partType = FigurePartType.WaistAccessory; break;
            case "cc": partType = FigurePartType.Coat; break;
            case "cp": partType = FigurePartType.ChestPrint; break;
            default: partType = default; return false;
        }
        return true;
    }

    public static FigurePartType GetFigurePartType(string partTypeString)
    {
        if (TryGetFigurePartType(partTypeString, out FigurePartType partType))
            return partType;

        throw new Exception($"Unknown figure part type '{partTypeString}'");
    }
    #endregion

    #region - Movement -
    private static readonly int[][] magicVectors =
    {
        new[] { -1000, -10000 }, // N
        new[] { 1000, -10000 },  // NE
        new[] { 10000, -1000 },  // E
        new[] { 10000, 1000 },   // SE
        new[] { 1000, 10000 },   // S
        new[] { -1000, 10000 },  // SW
        new[] { -10000, 1000 },  // W
        new[] { -10000, -1000 }  // NW
    };

    /// <summary>
    /// Gets a vector that can be used to face the specified direction
    /// regardless of the position of the user in the room.
    /// </summary>
    /// <param name="direction">The direction to face.</param>
    public static (int X, int Y) GetMagicVector(int direction)
    {
        var vector = magicVectors[direction % 8];
        return (vector[0], vector[1]);
    }

    public static (int X, int Y) GetMagicVector(Directions direction) => GetMagicVector((int)direction);

    /// <summary>
    /// Gets a vector that points in the specified direction by one tile space.
    /// </summary>
    public static (int X, int Y) GetDirectionVector(Directions direction)
    {
        return direction switch
        {
            Directions.North => (0, -1),
            Directions.NorthEast => (1, -1),
            Directions.East => (1, 0),
            Directions.SouthEast => (1, 1),
            Directions.South => (0, 1),
            Directions.SouthWest => (-1, 1),
            Directions.West => (-1, 0),
            Directions.NorthWest => (-1, -1),
            _ => (0, 0),
        };
    }

    /// <summary>
    /// Gets a vector that points in the specified direction by one tile space.
    /// </summary>
    public static (int X, int Y) GetDirectionVector(int direction) => GetDirectionVector((Directions)direction);
    #endregion

    #region - Room -
    public static int GetHeightFromCharacter(char c)
    {
        if ('0' <= c && c <= '9')
            return c - '0';
        else if ('a' <= c && c != 'x' && c <= 'z')
            return 10 + (c - 'a');
        else if ('A' <= c && c != 'X' && c <= 'Z')
            return 10 + (c - 'A');
        else
            return -110;
    }

    public static char GetCharacterFromHeight(int height)
    {
        if (0 <= height && height < 10)
            return (char)('0' + height);
        else if (10 <= height && height < 36)
            return (char)('a' + (height - 10));
        else
            return 'x';
    }
    #endregion

    #region - Text -
    private static readonly Dictionary<char, string> _altCharacterMap = new Dictionary<char, string>()
    {
        { '‡', "🚫" },
        { '|', "❤️" },
        { '¥', "★" },
        { 'ƒ', "🖤" },
        { '—', "🎵" },
        { 'ª', "💀" },
        { 'º', "⚡" },
        { 'µ', "☕" },
        { '±', "📱" },
        { '÷', "👎" },
        { '•', "👍" },
        { '¶', "💡" },
        { '‘', "🔒" },
        { '†', "💣" },
        { '¬', "🐟" },
        { '»', "♣️" }
    };

    public static IReadOnlyDictionary<char, string> GetAltCharacterMap() => _altCharacterMap.ToDictionary(x => x.Key, x => x.Value);

    public static string RenderText(string text)
    {
        var sb = new StringBuilder();
        foreach (char c in text)
        {
            if (_altCharacterMap.ContainsKey(c))
                sb.Append(_altCharacterMap[c]);
            else
                sb.Append(c);
        }
        return sb.ToString();
    }
    #endregion
}
