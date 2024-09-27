using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IBot"/>
public class Bot : Avatar, IBot
{
    public bool IsPublicBot => Type == AvatarType.PublicBot;
    public bool IsPrivateBot => Type == AvatarType.PrivateBot;

    public Gender Gender { get; set; }
    public Id OwnerId { get; set; }
    public string OwnerName { get; set; }
    public List<short> Skills { get; set; }
    IReadOnlyList<short> IBot.Data => Skills;

    public Bot(AvatarType type, Id id, int index)
        : base(type, id, index)
    {
        if (type is not (AvatarType.PublicBot or AvatarType.PrivateBot))
            throw new ArgumentException($"Invalid avatar type for Bot: {type}");

        Gender = Gender.Unisex;
        OwnerId = -1;
        OwnerName = "";
        Skills = [];
    }

    public Bot(AvatarType type, Id id, int index, in PacketReader p)
        : this(type, id, index)
    {
        if (p.Client == ClientType.Shockwave) return;

        if (type == AvatarType.PrivateBot)
        {
            Gender = H.ToGender(p.ReadString());
            OwnerId = p.ReadId();
            OwnerName = p.ReadString();
            Skills = [.. p.ReadShortArray()];
        }
    }

    public override void Compose(in PacketWriter p)
    {
        base.Compose(in p);

        if (p.Client == ClientType.Shockwave) return;

        if (Type == AvatarType.PrivateBot)
        {
            p.WriteString(Gender.ToClientString().ToLower());
            p.WriteId(OwnerId);
            p.WriteString(OwnerName);
            p.WriteShortArray(Skills);
        }
    }
}
