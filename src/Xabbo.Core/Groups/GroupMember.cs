using System;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IGroupMember"/>
public sealed class GroupMember : IGroupMember, IParserComposer<GroupMember>
{
    public GroupMemberType Type { get; set; }
    public Id Id { get; set; }
    public string Name { get; set; } = "";
    public string Figure { get; set; } = "";
    public DateTime Joined { get; set; }

    public GroupMember() { }

    private GroupMember(in PacketReader p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        Type = (GroupMemberType)p.ReadInt();
        Id = p.ReadId();
        Name = p.ReadString();
        Figure = p.ReadString();
        Joined = DateTime.Parse(p.ReadString());
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.WriteInt((int)Type);
        p.WriteId(Id);
        p.WriteString(Name);
        p.WriteString(Figure);
        p.WriteString(Joined.ToString()); // TODO Check string format.
    }

    static GroupMember IParser<GroupMember>.Parse(in PacketReader p) => new(in p);
}