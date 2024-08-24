using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public class RoomInfo : IRoomInfo, IComposer, IParser<RoomInfo>
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
    public long GroupId { get; set; }
    public string GroupName { get; set; }
    public string GroupBadge { get; set; }
    public string EventName { get; set; }
    public string EventDescription { get; set; }
    public int EventMinutesRemaining { get; set; }

    public RoomInfo()
    {
        Name =
        OwnerName =
        Description =
        OfficialRoomPicRef =
        GroupName =
        GroupBadge =
        EventName =
        EventDescription = string.Empty;
        Tags = [];
    }

    protected RoomInfo(in PacketReader p) : this()
    {
        Id = p.Read<Id>();
        Name = p.Read<string>();
        OwnerId = p.Read<Id>();
        OwnerName = p.Read<string>();
        Access = (RoomAccess)p.Read<int>();
        Users = p.Read<int>();
        MaxUsers = p.Read<int>();
        Description = p.Read<string>();
        Trading = (TradePermissions)p.Read<int>();
        Score = p.Read<int>();
        Ranking = p.Read<int>();
        Category = (RoomCategory)p.Read<int>();

        int n = p.Read<Length>();
        for (int i = 0; i < n; i++)
        {
            Tags.Add(p.Read<string>());
        }

        Flags = (RoomFlags)p.Read<int>();

        if (Flags.HasFlag(RoomFlags.HasOfficialRoomPic))
        {
            OfficialRoomPicRef = p.Read<string>();
        }

        if (Flags.HasFlag(RoomFlags.IsGroupHomeRoom))
        {
            GroupId = p.Read<Id>();
            GroupName = p.Read<string>();
            GroupBadge = p.Read<string>();
        }

        if (Flags.HasFlag(RoomFlags.HasEvent))
        {
            EventName = p.Read<string>();
            EventDescription = p.Read<string>();
            EventMinutesRemaining = p.Read<int>();
        }
    }

    public virtual void Compose(in PacketWriter p)
    {
        p.Write(Id);
        p.Write(Name);

        p.Write(OwnerId);
        p.Write(OwnerName);
        p.Write((int)Access);
        p.Write(Users);
        p.Write(MaxUsers);
        p.Write(Description);
        p.Write((int)Trading);
        p.Write(Score);
        p.Write(Ranking);
        p.Write((int)Category);

        p.Write<Length>(Tags.Count);
        foreach (string tag in Tags)
        {
            p.Write(tag);
        }

        p.Write((int)Flags);

        if (Flags.HasFlag(RoomFlags.HasOfficialRoomPic))
        {
            p.Write(OfficialRoomPicRef);
        }

        if (Flags.HasFlag(RoomFlags.IsGroupHomeRoom))
        {
            p.Write(GroupId);
            p.Write(GroupName);
            p.Write(GroupBadge);
        }

        if (Flags.HasFlag(RoomFlags.HasEvent))
        {
            p.Write(EventName);
            p.Write(EventDescription);
            p.Write(EventMinutesRemaining);
        }
    }

    public static RoomInfo Parse(in PacketReader packet) => new(in packet);
}
