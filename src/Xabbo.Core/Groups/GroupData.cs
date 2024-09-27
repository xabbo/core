using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IGroupData"/>
public sealed class GroupData : IGroupData, IParserComposer<GroupData>
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
    public bool IsFavourite { get; set; }
    public string Created { get; set; }
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

        Id = p.ReadId();
        IsGuild = p.ReadBool();
        Type = (GroupType)p.ReadInt();
        Name = p.ReadString();
        Description = p.ReadString();
        Badge = p.ReadString();
        HomeRoomId = p.ReadId();
        HomeRoomName = p.ReadString();
        MemberStatus = (GroupMemberStatus)p.ReadInt();
        MemberCount = p.ReadInt();
        IsFavourite = p.ReadBool();
        Created = p.ReadString();
        IsOwner = p.ReadBool();
        IsAdmin = p.ReadBool();
        OwnerName = p.ReadString();
        ShowInClient = p.ReadBool();
        CanDecorateHomeRoom = p.ReadBool();
        PendingRequests = p.ReadInt();
        HasForum = p.ReadBool();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.WriteId(Id);
        p.WriteBool(IsGuild);
        p.WriteInt((int)Type);
        p.WriteString(Name);
        p.WriteString(Description);
        p.WriteString(Badge);
        p.WriteId(HomeRoomId);
        p.WriteString(HomeRoomName);
        p.WriteInt((int)MemberStatus);
        p.WriteInt(MemberCount);
        p.WriteBool(IsFavourite);
        p.WriteString(Created);
        p.WriteBool(IsOwner);
        p.WriteBool(IsAdmin);
        p.WriteString(OwnerName);
        p.WriteBool(ShowInClient);
        p.WriteBool(CanDecorateHomeRoom);
        p.WriteInt(PendingRequests);
        p.WriteBool(HasForum);
    }

    static GroupData IParser<GroupData>.Parse(in PacketReader p) => new(in p);
}
