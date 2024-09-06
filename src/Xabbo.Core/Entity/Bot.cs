using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public class Bot : Entity, IBot
{
    public bool IsPublicBot => Type == EntityType.PublicBot;
    public bool IsPrivateBot => Type == EntityType.PrivateBot;

    public Gender Gender { get; set; }
    public Id OwnerId { get; set; }
    public string OwnerName { get; set; }
    public List<short> Skills { get; set; }
    IReadOnlyList<short> IBot.Data => Skills;

    public Bot(EntityType type, Id id, int index)
        : base(type, id, index)
    {
        if (type is not (EntityType.PublicBot or EntityType.PrivateBot))
            throw new ArgumentException($"Invalid entity type for Bot: {type}");

        Gender = Gender.Unisex;
        OwnerId = -1;
        OwnerName = string.Empty;
        Skills = [];
    }

    public Bot(EntityType type, Id id, int index, in PacketReader p)
        : this(type, id, index)
    {
        if (p.Client == ClientType.Shockwave) return;

        if (type == EntityType.PrivateBot)
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

        if (Type == EntityType.PrivateBot)
        {
            p.WriteString(Gender.ToShortString().ToLower());
            p.WriteId(OwnerId);
            p.WriteString(OwnerName);
            p.WriteShortArray(Skills);
        }
    }
}
