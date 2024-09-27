namespace Xabbo.Core;

/// <summary>
/// Represents the category of a room in the navigator.
/// </summary>
public enum RoomCategory
{
    None = -1,
    Party = 2,
    Games = 3,
    FansiteSquare = 5,
    HelpCenters = 6,
    PersonalSpace = 10,
    BuildingAndDecoration = 11,
    ChatAndDiscussion = 12,
    Trading = 14,
    Agencies = 16,
    RolePlaying = 17
}

public static partial class XabboEnumExtensions
{
    public static string ToFriendlyName(this RoomCategory category)
    {
        switch (category)
        {
            case RoomCategory.Party: return "Parties";
            case RoomCategory.Games: return "Games";
            case RoomCategory.FansiteSquare: return "Fansite Squares";
            case RoomCategory.HelpCenters: return "Help Centers";
            case RoomCategory.PersonalSpace: return "Personal Spaces";
            case RoomCategory.BuildingAndDecoration: return "Building and Decoration";
            case RoomCategory.ChatAndDiscussion: return "Chat and Discussion";
            case RoomCategory.Trading: return "Trading";
            case RoomCategory.Agencies: return "Agencies";
            case RoomCategory.RolePlaying: return "Role Playing";
            default: return category.ToString();
        }
    }
}
