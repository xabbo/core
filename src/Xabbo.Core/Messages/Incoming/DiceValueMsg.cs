using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record DiceValueMsg(Id Id, int Value) : IMessage<DiceValueMsg>
{
    static Identifier IMessage<DiceValueMsg>.Identifier => In.DiceValue;

    static DiceValueMsg IParser<DiceValueMsg>.Parse(in PacketReader p)
    {
        Id id;
        int value;

        if (p.Client is ClientType.Shockwave)
        {
            string[] fields = p.ReadContent().Split();
            if (fields.Length is < 1 or > 2)
                throw new Exception($"Invalid field count when parsing {nameof(DiceValueMsg)}.");
            if (!Id.TryParse(fields[0], out id))
                throw new FormatException($"Failed to parse {nameof(Id)} in {nameof(DiceValueMsg)}.");
            if (!int.TryParse(fields[1], out value))
                throw new FormatException($"Failed to parse {nameof(Value)} in {nameof(DiceValueMsg)}.");
            value %= (int)id;
        }
        else
        {
            id = p.ReadId();
            value = p.ReadInt();
        }

        return new(id, value);
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            p.WriteContent($"{Id} {Value + Id * 38}");
        }
        else
        {
            p.WriteId(Id);
            p.WriteInt(Value);
        }
    }
}