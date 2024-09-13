using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Xabbo.Core;

public static partial class H
{
    [GeneratedRegex(@"^[a-zA-Z0-9_\-=?!@:.,]{3,15}$")]
    private static partial Regex RegexAvatarValidator();

    public static string UserAgent { get; set; } = XabboConst.DefaultUserAgent;

    public static bool IsValidAvatarName(string avatarName) => RegexAvatarValidator().IsMatch(avatarName);

    public static ItemType ToItemType(string s)
    {
        if (s.Length != 1)
            throw new Exception($"Invalid item type: '{s}'.");

        ItemType type = (ItemType)s.ToLower()[0];
        if (!Enum.IsDefined(typeof(ItemType), type))
            throw new Exception($"Unknown item type: '{s}'.");

        return type;
    }

    public static ItemType ToItemType(int value) => value switch
    {
        0 => ItemType.Wall,
        1 => ItemType.Floor,
        _ => throw new Exception($"Unknown item type: {value}.")
    };

    public static ItemType ToItemType(char value) => value switch
    {
        's' => ItemType.Floor,
        'i' => ItemType.Wall,
        'b' => ItemType.Badge,
        'e' => ItemType.Effect,
        'r' => ItemType.Bot,
        _ => throw new Exception($"Unknown item type: '{value}'.")
    };

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
        if (!TryGetFigurePartType(partTypeString, out FigurePartType partType))
            throw new Exception($"Unknown figure part type '{partTypeString}'");

        return partType;
    }
    #endregion

    #region - Movement -
    private static readonly int[][] magicVectors =
    [
        [-1000, -10000], // N
        [1000, -10000],  // NE
        [10000, -1000],  // E
        [10000, 1000],   // SE
        [1000, 10000],   // S
        [-1000, 10000],  // SW
        [-10000, 1000],  // W
        [-10000, -1000]  // NW
    ];

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
    public static (int X, int Y) GetDirectionVector(Directions direction) => direction switch
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


    /// <summary>
    /// Gets a vector that points in the specified direction by one tile space.
    /// </summary>
    public static (int X, int Y) GetDirectionVector(int direction) => GetDirectionVector((Directions)direction);
    #endregion

    #region - Room -
    public static int GetHeightFromCharacter(char c)
    {
        if (c is >= '0' and <= '9')
            return c - '0';
        else if (c is >= 'a' and <= 'z' and not 'x')
            return 10 + (c - 'a');
        else if (c is >= 'A' and <= 'Z' and not 'X')
            return 10 + (c - 'A');
        else
            return -110;
    }

    public static char GetCharacterFromHeight(int height)
    {
        if (height is >= 0 and < 10)
            return (char)('0' + height);
        else if (height is >= 10 and < 36)
            return (char)('a' + (height - 10));
        else
            return 'x';
    }
    #endregion

    #region - Text -
    private static readonly Dictionary<char, string> _altCharacterMap = new()
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
            if (_altCharacterMap.TryGetValue(c, out string? value))
                sb.Append(value);
            else
                sb.Append(c);
        }
        return sb.ToString();
    }
    #endregion
}
