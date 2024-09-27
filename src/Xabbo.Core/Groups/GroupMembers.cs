using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IGroupMembers"/>
public sealed class GroupMembers : List<GroupMember>, IGroupMembers, IParserComposer<GroupMembers>
{
    public Id GroupId { get; set; }
    public string GroupName { get; set; } = "";
    public Id HomeRoomId { get; set; }
    public string BadgeCode { get; set; } = "";
    public int TotalEntries { get; set; }
    public bool IsAllowedToManage { get; set; }
    public int PageSize { get; set; }
    public int PageIndex { get; set; }
    public GroupMemberSearchType SearchType { get; set; }
    public string Filter { get; set; } = "";

    IGroupMember IReadOnlyList<IGroupMember>.this[int index] => this[index];
    IEnumerator<IGroupMember> IEnumerable<IGroupMember>.GetEnumerator() => GetEnumerator();

    public GroupMembers() { }

    private GroupMembers(in PacketReader p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        GroupId = p.ReadId();
        GroupName = p.ReadString();
        HomeRoomId = p.ReadId();
        BadgeCode = p.ReadString();
        TotalEntries = p.ReadInt();
        int n = p.ReadLength();
        for (int i = 0; i < n; i++)
            Add(p.Parse<GroupMember>());
        IsAllowedToManage = p.ReadBool();
        PageSize = p.ReadInt();
        PageIndex = p.ReadInt();
        SearchType = (GroupMemberSearchType)p.ReadInt();
        Filter = p.ReadString();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.WriteId(GroupId);
        p.WriteString(GroupName);
        p.WriteId(HomeRoomId);
        p.WriteString(BadgeCode);
        p.WriteInt(TotalEntries);
        p.ComposeArray<GroupMember>(this);
        p.WriteBool(IsAllowedToManage);
        p.WriteInt(PageSize);
        p.WriteInt(PageIndex);
        p.WriteInt((int)SearchType);
        p.WriteString(Filter);
    }

    static GroupMembers IParser<GroupMembers>.Parse(in PacketReader p) => new(in p);
}
