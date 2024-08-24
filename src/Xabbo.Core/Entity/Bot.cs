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
        if (type != EntityType.PublicBot &&
            type != EntityType.PrivateBot)
        {
            throw new ArgumentException($"Invalid entity type for Bot: {type}");
        }

        Gender = Gender.Unisex;
        OwnerId = -1;
        OwnerName = string.Empty;
        Skills = [];
    }

    public Bot(EntityType type, Id id, int index, in PacketReader p)
        : this(type, id, index)
    {
        if (type == EntityType.PrivateBot)
        {
            Gender = H.ToGender(p.Read<string>());
            OwnerId = p.Read<Id>();
            OwnerName = p.Read<string>();

            int n = p.Read<Length>();
            for (int i = 0; i < n; i++)
            {
                Skills.Add(p.Read<short>());
            }
        }
    }

    public override void Compose(in PacketWriter p)
    {
        base.Compose(in p);

        if (Type == EntityType.PrivateBot)
        {
            p.Write(Gender.ToShortString().ToLower());
            p.Write(OwnerId);
            p.Write(OwnerName);
            p.Write(Skills);
        }
    }
}
