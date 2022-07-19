using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public class RoomSettings : IComposable
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

    protected RoomSettings(IReadOnlyPacket packet)
        : this()
    {
        Id = packet.ReadLegacyLong();
        Name = packet.ReadString();
        Description = packet.ReadString();
        Access = (RoomAccess)packet.ReadInt();
        Category = (RoomCategory)packet.ReadInt();
        MaxVisitors = packet.ReadInt(); // maximumVisitors
        packet.ReadInt(); // maximumVisitorsLimit

        short n = packet.ReadLegacyShort();
        for (int i = 0; i < n; i++)
            Tags.Add(packet.ReadString());

        Trading = (TradePermissions)packet.ReadInt(); // tradeMode
        AllowPets = packet.ReadInt() == 1;
        AllowOthersPetsToEat = packet.ReadInt() == 1; // allowFoodConsume
        DisableRoomBlocking = packet.ReadInt() == 1; // allowWalkThrough
        HideWalls = packet.ReadInt() == 1;
        WallThickness = (Thickness)packet.ReadInt();
        FloorThickness = (Thickness)packet.ReadInt();
        Chat = ChatSettings.Parse(packet);

        EnlistByFurniContent = packet.ReadBool(); // allowNavigatorDynamicCats
        Moderation = ModerationSettings.Parse(packet);
    }

    /// <summary>
    /// Writes the values of this <see cref="RoomSettings"/> to the specified packet
    /// to be sent to the server with <see cref="Outgoing.SaveRoomSettings"/>.
    /// </summary>
    public void Compose(IPacket packet) => packet
        .WriteLegacyLong(Id)
        .WriteString(Name ?? string.Empty)
        .WriteString(Description ?? string.Empty)
        .WriteInt((int)Access)
        .WriteString(Password ?? string.Empty)
        .WriteInt(MaxVisitors)
        .WriteInt((int)Category)
        .WriteCollection(Tags)
        .WriteInt((int)Trading)
        .WriteBool(AllowPets)
        .WriteBool(AllowOthersPetsToEat)
        .WriteBool(DisableRoomBlocking)
        .WriteBool(HideWalls)
        .WriteInt((int)WallThickness)
        .WriteInt((int)FloorThickness)
        .Write(Moderation)
        .Write(Chat)
        .WriteBool(EnlistByFurniContent);

    public static RoomSettings Parse(IReadOnlyPacket packet) => new(packet);
}
