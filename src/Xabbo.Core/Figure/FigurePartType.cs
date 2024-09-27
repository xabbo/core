using System;

namespace Xabbo.Core;

/// <summary>
/// Represents a part type in a figure.
/// </summary>
public enum FigurePartType
{
    Hair,
    HairBelow,
    Body,
    Head,
    Chest,
    LeftSleeve,
    RightSleeve,
    LeftHand,
    RightHand,
    LeftHandItem,
    RightHandItem,
    Legs,
    Shoes,
    Hat,
    HeadAccessory,
    Eyes,
    EyeAccessory,
    Face,
    FaceAccessory,
    ChestAccessory,
    WaistAccessory,
    Coat,
    LeftCoatSleeve,
    RightCoatSleeve,
    ChestPrint
}

public static partial class XabboEnumExtensions
{
    /// <summary>
    /// Converts the figure part type to its short 2-letter string representation.
    /// </summary>
    public static string ToShortString(this FigurePartType figurePartType)
    {
        return figurePartType switch
        {
            FigurePartType.Hair => "hr",
            FigurePartType.Head => "hd",
            FigurePartType.Chest => "ch",
            FigurePartType.Legs => "lg",
            FigurePartType.Shoes => "sh",
            FigurePartType.Hat => "ha",
            FigurePartType.HeadAccessory => "he",
            FigurePartType.EyeAccessory => "ea",
            FigurePartType.FaceAccessory => "fa",
            FigurePartType.ChestAccessory => "ca",
            FigurePartType.WaistAccessory => "wa",
            FigurePartType.Coat => "cc",
            FigurePartType.ChestPrint => "cp",
            _ => throw new ArgumentException($"Unknown figure part type: {figurePartType}", nameof(figurePartType)),
        };
    }
}
