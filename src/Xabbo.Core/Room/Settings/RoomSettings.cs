using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core;

public class RoomSettings : IParserComposer<RoomSettings>
{
    public Id Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public RoomAccess Access { get; set; }
    public string Password { get; set; } = "";
    public int MaxVisitors { get; set; }
    public int AbsoluteMaxVisitors { get; set; }
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
        Id = p.ReadId();
        Name = p.ReadString();
        Description = p.ReadString();
        Access = (RoomAccess)p.ReadInt();
        Category = (RoomCategory)p.ReadInt();
        MaxVisitors = p.ReadInt(); // maximumVisitors
        AbsoluteMaxVisitors = p.ReadInt(); // maximumVisitorsLimit

        Tags = [..p.ReadStringArray()];

        Trading = (TradePermissions)p.ReadInt(); // tradeMode
        AllowPets = p.ReadInt() == 1;
        AllowOthersPetsToEat = p.ReadInt() == 1; // allowFoodConsume
        DisableRoomBlocking = p.ReadInt() == 1; // allowWalkThrough
        HideWalls = p.ReadInt() == 1;
        WallThickness = (Thickness)p.ReadInt();
        FloorThickness = (Thickness)p.ReadInt();
        Chat = p.Parse<ChatSettings>();

        EnlistByFurniContent = p.ReadBool(); // allowNavigatorDynamicCats
        Moderation = p.Parse<ModerationSettings>();
    }

    /// <summary>
    /// Writes the values of this <see cref="RoomSettings"/> to the specified packet
    /// to be sent to the server with <see cref="Out.SaveRoomSettings"/>.
    /// </summary>
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteString(Name ?? string.Empty);
        p.WriteString(Description ?? string.Empty);
        p.WriteInt((int)Access);
        p.WriteString(Password ?? string.Empty);
        p.WriteInt(MaxVisitors);
        p.WriteInt((int)Category);
        p.WriteStringArray(Tags);
        p.WriteInt((int)Trading);
        p.WriteBool(AllowPets);
        p.WriteBool(AllowOthersPetsToEat);
        p.WriteBool(DisableRoomBlocking);
        p.WriteBool(HideWalls);
        p.WriteInt((int)WallThickness);
        p.WriteInt((int)FloorThickness);
        p.Compose(Moderation);
        p.Compose(Chat);
        p.WriteBool(EnlistByFurniContent);
    }

    static RoomSettings IParser<RoomSettings>.Parse(in PacketReader p) => new(p);
}
