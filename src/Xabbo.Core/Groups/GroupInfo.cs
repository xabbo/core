using System;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class GroupInfo : IGroupInfo
{
    public Id Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BadgeCode { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = string.Empty;
    public string SecondaryColor { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }
    public Id OwnerId { get; set; }
    public bool HasForum { get; set; }

    public GroupInfo() { }

    private GroupInfo(in PacketReader p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        Id = p.Read<Id>();
        Name = p.Read<string>();
        BadgeCode = p.Read<string>();
        PrimaryColor = p.Read<string>();
        SecondaryColor = p.Read<string>();
        IsFavorite = p.Read<bool>();
        OwnerId = p.Read<Id>();
        HasForum = p.Read<bool>();
    }

    public void Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.Write(Id);
        p.Write(Name);
        p.Write(BadgeCode);
        p.Write(PrimaryColor);
        p.Write(SecondaryColor);
        p.Write(IsFavorite);
        p.Write(OwnerId);
        p.Write(HasForum);
    }

    public static GroupInfo Parse(in PacketReader p) => new(in p);
}
