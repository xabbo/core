using System;

namespace Xabbo.Core
{
    public enum RoomCategory
    {
        ChatAndDiscussion = 0x0c,
        PersonalSpace = 0x0a,
        Agencies = 0x10,
        Games = 0x03,
        BuildingAndDecoration = 0x0b,
        Trading = 0x0e,
        Party = 0x02,
        RolePlaying = 0x11,
        FansiteSquare = 0x05,
        HelpCenters = 0x06
    }

    public static partial class EnumExtensions
    {
        public static string ToFriendlyName(this RoomCategory category)
        {
            switch (category)
            {
                case RoomCategory.ChatAndDiscussion: return "Chat and Discussion";
                case RoomCategory.PersonalSpace: return "Personal Spaces";
                case RoomCategory.Agencies: return "Agencies";
                case RoomCategory.Games: return "Games";
                case RoomCategory.BuildingAndDecoration: return "Building and Decoration";
                case RoomCategory.Trading: return "Trading";
                case RoomCategory.Party: return "Parties";
                case RoomCategory.RolePlaying: return "Role Playing";
                case RoomCategory.FansiteSquare: return "Fansite Squares";
                case RoomCategory.HelpCenters: return "Help Centers";
                default: return category.ToString();
            }
        }
    }
}
