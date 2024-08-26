using System;
using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class GroupMember : IGroupMember, IComposer, IParser<GroupMember>
{
    public GroupMemberType Type { get; set; }
    public Id Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Figure { get; set; } = string.Empty;
    public DateTime Joined { get; set; }

    public GroupMember() { }

    private GroupMember(in PacketReader p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        Type = (GroupMemberType)p.Read<int>();
        Id = p.Read<Id>();
        Name = p.Read<string>();
        Figure = p.Read<string>();
        Joined = DateTime.Parse(p.Read<string>());
    }

    public void Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.Write((int)Type);
        p.Write(Id);
        p.Write(Name);
        p.Write(Figure);
        p.Write(Joined);
    }

    public static GroupMember Parse(in PacketReader p) => new(in p);
}