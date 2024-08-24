using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class GroupData : IGroupData, IComposer, IParser<GroupData>
{
    public static GroupData Parse(in PacketReader packet) => new(in packet);

    public Id Id { get; set; }
    public bool IsGuild { get; set; }
    public GroupType Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Badge { get; set; }
    public Id HomeRoomId { get; set; }
    public string HomeRoomName { get; set; }
    public GroupMemberStatus MemberStatus { get; set; }
    public int MemberCount { get; set; }
    public bool IsFavourite { get; set; } public string Created { get; set; }
    public bool IsOwner { get; set; }
    public bool IsAdmin { get; set; }
    public string OwnerName { get; set; }
    public bool ShowInClient { get; set; }
    public bool CanDecorateHomeRoom { get; set; }
    public int PendingRequests { get; set; }
    public bool HasForum { get; set; }

    private GroupData(in PacketReader packet)
    {
        Id = packet.Read<Id>();
        IsGuild = packet.Read<bool>();
        Type = (GroupType)packet.Read<int>();
        Name = packet.Read<string>();
        Description = packet.Read<string>();
        Badge = packet.Read<string>();
        HomeRoomId = packet.Read<Id>();
        HomeRoomName = packet.Read<string>();
        MemberStatus = (GroupMemberStatus)packet.Read<int>();
        MemberCount = packet.Read<int>();
        IsFavourite = packet.Read<bool>();
        Created = packet.Read<string>();
        IsOwner = packet.Read<bool>();
        IsAdmin = packet.Read<bool>();
        OwnerName = packet.Read<string>();
        ShowInClient = packet.Read<bool>();
        CanDecorateHomeRoom = packet.Read<bool>();
        PendingRequests = packet.Read<int>();
        HasForum = packet.Read<bool>();
    }

    public void Compose(in PacketWriter p)
    {
        p.Write(Id);
        p.Write(IsGuild);
        p.Write((int)Type);
        p.Write(Name);
        p.Write(Description);
        p.Write(Badge);
        p.Write(HomeRoomId);
        p.Write(HomeRoomName);
        p.Write((int)MemberStatus);
        p.Write(MemberCount);
        p.Write(IsFavourite);
        p.Write(Created);
        p.Write(IsOwner);
        p.Write(IsAdmin);
        p.Write(OwnerName);
        p.Write(ShowInClient);
        p.Write(CanDecorateHomeRoom);
        p.Write(PendingRequests);
        p.Write(HasForum);
    }
}
