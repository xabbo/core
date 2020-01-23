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
            switch (figurePartType)
            {
                case FigurePartType.Hair: return "hr";
                case FigurePartType.Head: return "hd";
                case FigurePartType.Chest: return "ch";
                case FigurePartType.Legs: return "lg";
                case FigurePartType.Shoes: return "sh";
                case FigurePartType.Hat: return "ha";
                case FigurePartType.HeadAccessory: return "he";
                case FigurePartType.EyeAccessory: return "ea";
                case FigurePartType.FaceAccessory: return "fa";
                case FigurePartType.ChestAccessory: return "ca";
                case FigurePartType.WaistAccessory: return "wa";
                case FigurePartType.Coat: return "cc";
                case FigurePartType.ChestPrint: return "cp";
                default: return null;
            }
        }
    }
}
