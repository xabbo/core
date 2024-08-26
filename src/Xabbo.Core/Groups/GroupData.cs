using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class GroupData : IGroupData, IComposer, IParser<GroupData>
{
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

    private GroupData(in PacketReader p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        Id = p.Read<Id>();
        IsGuild = p.Read<bool>();
        Type = (GroupType)p.Read<int>();
        Name = p.Read<string>();
        Description = p.Read<string>();
        Badge = p.Read<string>();
        HomeRoomId = p.Read<Id>();
        HomeRoomName = p.Read<string>();
        MemberStatus = (GroupMemberStatus)p.Read<int>();
        MemberCount = p.Read<int>();
        IsFavourite = p.Read<bool>();
        Created = p.Read<string>();
        IsOwner = p.Read<bool>();
        IsAdmin = p.Read<bool>();
        OwnerName = p.Read<string>();
        ShowInClient = p.Read<bool>();
        CanDecorateHomeRoom = p.Read<bool>();
        PendingRequests = p.Read<int>();
        HasForum = p.Read<bool>();
    }

    public void Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

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

    public static GroupData Parse(in PacketReader p) => new(in p);
}
