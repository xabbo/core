using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class GroupMembers : List<GroupMember>, IGroupMembers, IComposer, IParser<GroupMembers>
{
    public Id GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public long HomeRoomId { get; set; }
    public string BadgeCode { get; set; } = string.Empty;
    public int TotalEntries { get; set; }
    public bool IsAllowedToManage { get; set; }
    public int PageSize { get; set; }
    public int PageIndex { get; set; }
    public GroupMemberSearchType SearchType { get; set; }
    public string Filter { get; set; } = string.Empty;

    IGroupMember IReadOnlyList<IGroupMember>.this[int index] => this[index];
    IEnumerator<IGroupMember> IEnumerable<IGroupMember>.GetEnumerator() => GetEnumerator();

    public GroupMembers() { }

    private GroupMembers(in PacketReader p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        GroupId = p.Read<Id>();
        GroupName = p.Read<string>();
        HomeRoomId = p.Read<Id>();
        BadgeCode = p.Read<string>();
        TotalEntries = p.Read<int>();
        int n = p.Read<Length>();
        for (int i = 0; i < n; i++)
            Add(p.Parse<GroupMember>());
        IsAllowedToManage = p.Read<bool>();
        PageSize = p.Read<int>();
        PageIndex = p.Read<int>();
        SearchType = (GroupMemberSearchType)p.Read<int>();
        Filter = p.Read<string>();
    }

    public void Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.Write(GroupId);
        p.Write(GroupName);
        p.Write(HomeRoomId);
        p.Write(BadgeCode);
        p.Write(TotalEntries);
        p.Write<Length>(Count);
        foreach (var member in this)
            p.Write(member);
        p.Write(IsAllowedToManage);
        p.Write(PageSize);
        p.Write(PageIndex);
        p.Write(SearchType);
        p.Write(Filter);
    }

    public static GroupMembers Parse(in PacketReader p) => new(in p);
}
