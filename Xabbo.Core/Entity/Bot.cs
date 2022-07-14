using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public class Bot : Entity, IBot
{
    public bool IsPublicBot => Type == EntityType.PublicBot;
    public bool IsPrivateBot => Type == EntityType.PrivateBot;

    public Gender Gender { get; set; }
    public long OwnerId { get; set; }
    public string OwnerName { get; set; }
    public List<short> Skills { get; set; }
    IReadOnlyList<short> IBot.Data => Skills;

    public Bot(EntityType type, long id, int index)
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
        Skills = new List<short>();
    }

    public Bot(EntityType type, long id, int index, IReadOnlyPacket packet)
        : this(type, id, index)
    {
        if (type == EntityType.PrivateBot)
        {
            Gender = H.ToGender(packet.ReadString());
            OwnerId = packet.ReadLegacyLong();
            OwnerName = packet.ReadString();

            short n = packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
            {
                Skills.Add(packet.ReadShort());
            }
        }
    }

    public override void Compose(IPacket packet)
    {
        base.Compose(packet);

        if (Type == EntityType.PrivateBot)
        {
            packet
                .WriteString(Gender.ToShortString().ToLower())
                .WriteLegacyLong(OwnerId)
                .WriteString(OwnerName);

            packet.WriteLegacyShort((short)Skills.Count);
            foreach (short value in Skills)
            {
                packet.WriteShort(value);
            }
        }
    }
}
