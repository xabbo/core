using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IRoomInfo"/>
public class RoomInfo : IRoomInfo, IParserComposer<RoomInfo>
{
    public Id Id { get; set; }
    public string Name { get; set; }
    public Id OwnerId { get; set; }
    public string OwnerName { get; set; }
    public RoomAccess Access { get; set; }
    public bool IsOpen => Access == RoomAccess.Open;
    public bool IsDoorbell => Access == RoomAccess.Doorbell;
    public bool IsLocked => Access == RoomAccess.Password;
    public bool IsInvisible => Access == RoomAccess.Invisible;
    public int Users { get; set; }
    public int MaxUsers { get; set; }
    public string Description { get; set; }
    public TradePermissions Trading { get; set; }
    public int Score { get; set; }
    public int Ranking { get; set; }
    public RoomCategory Category { get; set; }
    public List<string> Tags { get; set; }
    IReadOnlyList<string> IRoomInfo.Tags => Tags;

    public RoomFlags Flags { get; set; }
    public bool HasOfficialRoomPic => Flags.HasFlag(RoomFlags.HasOfficialRoomPic);
    public bool IsGroupRoom => Flags.HasFlag(RoomFlags.IsGroupHomeRoom);
    public bool HasEvent => Flags.HasFlag(RoomFlags.HasEvent);
    public bool ShowOwnerName => Flags.HasFlag(RoomFlags.ShowOwnerName);
    public bool AllowPets => Flags.HasFlag(RoomFlags.AllowPets);

    public string OfficialRoomPicRef { get; set; }
    public Id GroupId { get; set; }
    public string GroupName { get; set; }
    public string GroupBadge { get; set; }
    public string EventName { get; set; }
    public string EventDescription { get; set; }
    public int EventMinutesRemaining { get; set; }

    // Origins
    public bool CanOthersMoveFurni { get; set; }
    public int AbsoluteMaxUsers { get; set; }
    public string Model { get; set; } = "";
    public int Alert { get; set; }

    public RoomInfo()
    {
        Name =
        OwnerName =
        Description =
        OfficialRoomPicRef =
        GroupName =
        GroupBadge =
        EventName =
        EventDescription = "";
        Tags = [];
    }

    protected RoomInfo(in PacketReader p) : this()
    {
        switch (p.Client)
        {
            case ClientType.Unity or ClientType.Flash:
                ParseModern(in p);
                break;
            case ClientType.Shockwave:
                ParseOrigins(in p);
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }
    }

    private void ParseModern(in PacketReader p)
    {
        Id = p.ReadId();
        Name = p.ReadString();
        OwnerId = p.ReadId();
        OwnerName = p.ReadString();
        Access = (RoomAccess)p.ReadInt();
        Users = p.ReadInt();
        MaxUsers = p.ReadInt();
        Description = p.ReadString();
        Trading = (TradePermissions)p.ReadInt();
        Score = p.ReadInt();
        Ranking = p.ReadInt();
        Category = (RoomCategory)p.ReadInt();

        Tags = [.. p.ReadStringArray()];

        Flags = (RoomFlags)p.ReadInt();

        if (Flags.HasFlag(RoomFlags.HasOfficialRoomPic))
        {
            OfficialRoomPicRef = p.ReadString();
        }

        if (Flags.HasFlag(RoomFlags.IsGroupHomeRoom))
        {
            GroupId = p.ReadId();
            GroupName = p.ReadString();
            GroupBadge = p.ReadString();
        }

        if (Flags.HasFlag(RoomFlags.HasEvent))
        {
            EventName = p.ReadString();
            EventDescription = p.ReadString();
            EventMinutesRemaining = p.ReadInt();
        }
    }

    private void ParseOrigins(in PacketReader p)
    {
        CanOthersMoveFurni = p.ReadBool();
        Access = (RoomAccess)p.ReadInt();
        Id = p.ReadId();
        OwnerId = -1;
        OwnerName = p.ReadString();
        Model = p.ReadString();
        Name = p.ReadString();
        Description = p.ReadString();
        if (p.ReadBool())
            Flags |= RoomFlags.ShowOwnerName;
        Trading = p.ReadInt() switch
        {
            0 => TradePermissions.NotAllowed,
            1 => TradePermissions.Allowed,
            int value => throw new Exception($"Unknown trade permissions on Origins: {value}.")
        };
        Alert = p.ReadInt();
        MaxUsers = p.ReadInt();
        AbsoluteMaxUsers = p.ReadInt();
    }

    void IComposer.Compose(in PacketWriter p) => Compose(in p);
    protected virtual void Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
            ComposeOrigins(in p);
        else
            ComposeModern(in p);
    }

    private void ComposeModern(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteString(Name);

        p.WriteId(OwnerId);
        p.WriteString(OwnerName);
        p.WriteInt((int)Access);
        p.WriteInt(Users);
        p.WriteInt(MaxUsers);
        p.WriteString(Description);
        p.WriteInt((int)Trading);
        p.WriteInt(Score);
        p.WriteInt(Ranking);
        p.WriteInt((int)Category);

        p.WriteLength((Length)Tags.Count);
        foreach (string tag in Tags)
            p.WriteString(tag);

        p.WriteInt((int)Flags);
        if (Flags.HasFlag(RoomFlags.HasOfficialRoomPic))
            p.WriteString(OfficialRoomPicRef);

        if (Flags.HasFlag(RoomFlags.IsGroupHomeRoom))
        {
            p.WriteId(GroupId);
            p.WriteString(GroupName);
            p.WriteString(GroupBadge);
        }

        if (Flags.HasFlag(RoomFlags.HasEvent))
        {
            p.WriteString(EventName);
            p.WriteString(EventDescription);
            p.WriteInt(EventMinutesRemaining);
        }
    }

    protected void ComposeOrigins(in PacketWriter p)
    {
        p.WriteBool(CanOthersMoveFurni);
        p.WriteInt((int)Access);
        p.WriteId(Id);
        p.WriteString(OwnerName);
        p.WriteString(Model);
        p.WriteString(Name);
        p.WriteString(Description);
        p.WriteBool((Flags & RoomFlags.ShowOwnerName) > 0);
        p.WriteInt(Trading switch
        {
            TradePermissions.NotAllowed => 0,
            TradePermissions.Allowed => 1,
            _ => throw new Exception($"Invalid trade permissions on Origins: {Trading}.")
        });
        p.WriteInt(Alert);
        p.WriteInt(MaxUsers);
        p.WriteInt(AbsoluteMaxUsers);
    }

    static RoomInfo IParser<RoomInfo>.Parse(in PacketReader p) => new(in p);
}
