using System;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class GroupInfo : IGroupInfo
{
    public static GroupInfo Parse(in PacketReader packet) => new GroupInfo(in packet);

    public Id Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BadgeCode { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = string.Empty;
    public string SecondaryColor { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }
    public Id OwnerId { get; set; }
    public bool HasForum { get; set; }

    public GroupInfo() { }

    private GroupInfo(in PacketReader packet)
    {
        Id = packet.Read<Id>();
        Name = packet.Read<string>();
        BadgeCode = packet.Read<string>();
        PrimaryColor = packet.Read<string>();
        SecondaryColor = packet.Read<string>();
        IsFavorite = packet.Read<bool>();
        OwnerId = packet.Read<Id>();
        HasForum = packet.Read<bool>();
    }

    public void Compose(in PacketWriter p)
    {
        p.Write(Id);
        p.Write(Name);
        p.Write(BadgeCode);
        p.Write(PrimaryColor);
        p.Write(SecondaryColor);
        p.Write(IsFavorite);
        p.Write(OwnerId);
        p.Write(HasForum);
    }
}
