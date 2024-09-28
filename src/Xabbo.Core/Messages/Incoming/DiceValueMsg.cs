using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when the value of a dice is updated.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.DiceValue"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.DICE_VALUE"/></item>
/// </list>
/// </summary>
/// <param name="Id">The ID of the dice.</param>
/// <param name="Value">The updated dice value.</param>
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
            if (fields.Length >= 2)
            {
                if (!int.TryParse(fields[1], out value))
                    throw new FormatException($"Failed to parse {nameof(Value)} in {nameof(DiceValueMsg)}.");
                value %= (int)id;
            }
            else
            {
                value = -1;
            }
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
