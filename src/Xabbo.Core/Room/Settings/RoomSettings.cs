using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core;

/// <summary>
/// Defines the settings for a room.
/// </summary>
public class RoomSettings : IParserComposer<RoomSettings>
{
    /// <summary>
    /// The ID of the room.
    /// </summary>
    public Id Id { get; set; }

    /// <summary>
    /// The name of the room.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// The description of the room.
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// The access mode of the room.
    /// </summary>
    public RoomAccess Access { get; set; }

    /// <summary>
    /// The password for the room if <see cref="Access"/> is <see cref="RoomAccess.Password"/>.
    /// </summary>
    public string Password { get; set; } = "";

    /// <summary>
    /// The maximum number of users allowed in the room.
    /// </summary>
    public int MaxVisitors { get; set; }

    public int AbsoluteMaxVisitors { get; set; }

    /// <summary>
    /// The category of the room in the navigator.
    /// </summary>
    public RoomCategory Category { get; set; }

    /// <summary>
    /// A list of tags that can be used to search for the room in the navigator.
    /// </summary>
    public List<string> Tags { get; set; }

    /// <summary>
    /// The trading permissions for the room.
    /// </summary>
    public TradePermissions Trading { get; set; }

    /// <summary>
    /// Whether others users' pets are allowed in the room.
    /// </summary>
    public bool AllowPets { get; set; }

    /// <summary>
    /// Whether other users' pets are allowed to eat food placed in the room.
    /// </summary>
    public bool AllowOthersPetsToEat { get; set; }

    /// <summary>
    /// Whether to disable room blocking and allow users to walk through each other.
    /// </summary>
    public bool DisableRoomBlocking { get; set; }

    /// <summary>
    /// Whether to hide the walls.
    /// </summary>
    public bool HideWalls { get; set; }

    /// <summary>
    /// The thickness of the walls.
    /// </summary>
    public Thickness WallThickness { get; set; }

    /// <summary>
    /// The thickness of the floor.
    /// </summary>
    public Thickness FloorThickness { get; set; }

    /// <summary>
    /// The moderation settings for the room.
    /// </summary>
    public ModerationSettings Moderation { get; set; }

    /// <summary>
    /// The chat settings for the room.
    /// </summary>
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

        Tags = [.. p.ReadStringArray()];

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
        p.WriteString(Name ?? "");
        p.WriteString(Description ?? "");
        p.WriteInt((int)Access);
        p.WriteString(Password ?? "");
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
