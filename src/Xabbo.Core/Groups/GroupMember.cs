using System;
using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class GroupMember : IGroupMember, IComposer, IParser<GroupMember>
{
    public static GroupMember Parse(in PacketReader packet) => new(in packet);

    public GroupMemberType Type { get; set; }
    public Id Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Figure { get; set; } = string.Empty;
    public DateTime Joined { get; set; }

    public GroupMember() { }

    private GroupMember(in PacketReader packet)
    {
        Type = (GroupMemberType)packet.Read<int>();
        Id = packet.Read<Id>();
        Name = packet.Read<string>();
        Figure = packet.Read<string>();
        Joined = DateTime.Parse(packet.Read<string>());
    }

    public void Compose(in PacketWriter p)
    {
        p.Write((int)Type);
        p.Write(Id);
        p.Write(Name);
        p.Write(Figure);
        p.Write(Joined);
    }
}