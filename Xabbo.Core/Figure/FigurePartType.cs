using System;

namespace Xabbo.Core
{
    public enum FigurePartType
    {
        Hair,
        Head,
        Chest,
        Legs,
        Shoes,
        Hat,
        HeadAccessory,
        EyeAccessory,
        FaceAccessory,
        ChestAccessory,
        WaistAccessory,
        Coat,
        ChestPrint
    }

    public static partial class XabboEnumExtensions
    {
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
}
