using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core;

public class RoomSettings : IComposer, IParser<RoomSettings>
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public RoomAccess Access { get; set; }
    public string Password { get; set; } = "";
    public int MaxVisitors { get; set; }
    public int UnknownIntA { get; set; }
    public RoomCategory Category { get; set; }
    public List<string> Tags { get; set; }
    public TradePermissions Trading { get; set; }

    public bool AllowPets { get; set; }
    public bool AllowOthersPetsToEat { get; set; }
    public bool DisableRoomBlocking { get; set; }
    public bool HideWalls { get; set; }
    public Thickness WallThickness { get; set; }
    public Thickness FloorThickness { get; set; }

    public ModerationSettings Moderation { get; set; }
    public ChatSettings Chat { get; set; }

    public bool EnlistByFurniContent { get; set; }

    public RoomSettings()
    {
        Tags = [];
        Moderation = new();
        Chat = new();
    }

    protected RoomSettings(in PacketReader p) : this()
    {
        Id = p.Read<Id>();
        Name = p.Read<string>();
        Description = p.Read<string>();
        Access = (RoomAccess)p.Read<int>();
        Category = (RoomCategory)p.Read<int>();
        MaxVisitors = p.Read<int>(); // maximumVisitors
        p.Read<int>(); // maximumVisitorsLimit

        int n = p.Read<Length>();
        for (int i = 0; i < n; i++)
            Tags.Add(p.Read<string>());

        Trading = (TradePermissions)p.Read<int>(); // tradeMode
        AllowPets = p.Read<int>() == 1;
        AllowOthersPetsToEat = p.Read<int>() == 1; // allowFoodConsume
        DisableRoomBlocking = p.Read<int>() == 1; // allowWalkThrough
        HideWalls = p.Read<int>() == 1;
        WallThickness = (Thickness)p.Read<int>();
        FloorThickness = (Thickness)p.Read<int>();
        Chat = ChatSettings.Parse(p);

        EnlistByFurniContent = p.Read<bool>(); // allowNavigatorDynamicCats
        Moderation = ModerationSettings.Parse(p);
    }

    /// <summary>
    /// Writes the values of this <see cref="RoomSettings"/> to the specified packet
    /// to be sent to the server with <see cref="Out.SaveRoomSettings"/>.
    /// </summary>
    public void Compose(in PacketWriter p)
    {
        p.Write(Id);
        p.Write(Name ?? string.Empty);
        p.Write(Description ?? string.Empty);
        p.Write((int)Access);
        p.Write(Password ?? string.Empty);
        p.Write(MaxVisitors);
        p.Write((int)Category);
        p.Write(Tags);
        p.Write((int)Trading);
        p.Write(AllowPets);
        p.Write(AllowOthersPetsToEat);
        p.Write(DisableRoomBlocking);
        p.Write(HideWalls);
        p.Write((int)WallThickness);
        p.Write((int)FloorThickness);
        p.Write(Moderation);
        p.Write(Chat);
        p.Write(EnlistByFurniContent);
    }

    public static RoomSettings Parse(in PacketReader p) => new(p);
}
