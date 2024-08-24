using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core;

public class RoomSettings : IComposer, IParser<RoomSettings>
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RoomAccess Access { get; set; }
    public string Password { get; set; } = string.Empty;
    public int MaxVisitors { get; set; }
    public int UnknownIntA { get; set; }
    public RoomCategory Category { get; set; }
    public List<string> Tags { get; set; } = new();
    public TradePermissions Trading { get; set; }

    public bool AllowPets { get; set; }
    public bool AllowOthersPetsToEat { get; set; }
    public bool DisableRoomBlocking { get; set; }
    public bool HideWalls { get; set; }
    public Thickness WallThickness { get; set; }
    public Thickness FloorThickness { get; set; }

    public ModerationSettings Moderation { get; set; } = new();
    public ChatSettings Chat { get; set; } = new();

    public bool EnlistByFurniContent { get; set; }

    public RoomSettings() { }

    protected RoomSettings(in PacketReader packet) : this()
    {
        Id = packet.Read<Id>();
        Name = packet.Read<string>();
        Description = packet.Read<string>();
        Access = (RoomAccess)packet.Read<int>();
        Category = (RoomCategory)packet.Read<int>();
        MaxVisitors = packet.Read<int>(); // maximumVisitors
        packet.Read<int>(); // maximumVisitorsLimit

        int n = packet.Read<Length>();
        for (int i = 0; i < n; i++)
            Tags.Add(packet.Read<string>());

        Trading = (TradePermissions)packet.Read<int>(); // tradeMode
        AllowPets = packet.Read<int>() == 1;
        AllowOthersPetsToEat = packet.Read<int>() == 1; // allowFoodConsume
        DisableRoomBlocking = packet.Read<int>() == 1; // allowWalkThrough
        HideWalls = packet.Read<int>() == 1;
        WallThickness = (Thickness)packet.Read<int>();
        FloorThickness = (Thickness)packet.Read<int>();
        Chat = ChatSettings.Parse(packet);

        EnlistByFurniContent = packet.Read<bool>(); // allowNavigatorDynamicCats
        Moderation = ModerationSettings.Parse(packet);
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

    public static RoomSettings Parse(in PacketReader packet) => new(packet);
}
