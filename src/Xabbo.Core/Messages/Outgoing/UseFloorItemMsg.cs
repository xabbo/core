using System;

using Xabbo.Messages;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when using a floor item.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Xabbo.Messages.Flash.Out.UseFurniture"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.SETSTUFFDATA"/></item>
/// </list>
/// </summary>
/// <param name="Id">The ID of the floor item.</param>
/// <param name="State">The state of the floor item. Appears to be unused - items are always toggled between states.</param>
public sealed record UseFloorItemMsg(Id Id, int State = 0) : IMessage<UseFloorItemMsg>
{
    static Identifier IMessage<UseFloorItemMsg>.Identifier => default;
    static Identifier[] IMessage<UseFloorItemMsg>.Identifiers { get; } = [
        Xabbo.Messages.Flash.Out.UseFurniture,
        Xabbo.Messages.Shockwave.Out.SETSTUFFDATA,
    ];
    static bool IMessage<UseFloorItemMsg>.UseTargetedIdentifiers => true;

    Identifier IMessage.GetIdentifier(ClientType client) => client switch
    {
        ClientType.Flash => Xabbo.Messages.Flash.Out.UseFurniture,
        ClientType.Shockwave => Xabbo.Messages.Shockwave.Out.SETSTUFFDATA,
        _ => throw new UnsupportedClientException(client),
    };

    static UseFloorItemMsg IParser<UseFloorItemMsg>.Parse(in PacketReader p)
    {
        Id id; int state;

        switch (p.Client)
        {
            case ClientType.Unity or ClientType.Flash:
                id = p.ReadId();
                state = p.ReadInt();
                break;
            case ClientType.Shockwave:
                string strId = p.ReadString();
                if (!Id.TryParse(strId, out id))
                    throw new FormatException($"Failed to parse ID when parsing UseFloorItemMsg: '{strId}'.");
                string strState = p.ReadString();
                if (strState is "C" or "OFF" or "FALSE") // closed, off
                    state = 0;
                else if (strState is "O" or "ON" or "TRUE") // open, on
                    state = 1;
                else
                {
                    if (!int.TryParse(strState, out state))
                        throw new FormatException($"Failed to parse State in UseFloorItemMsg: '{strState}'.");
                }
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }

        return new(id, state);
    }

    void IComposer.Compose(in PacketWriter p)
    {
        switch (p.Client)
        {
            case ClientType.Unity or ClientType.Flash:
                p.WriteId(Id);
                p.WriteInt(State);
                break;
            case ClientType.Shockwave:
                p.WriteString(Id.ToString());
                // Shockwave has strings such as "ON"/"OFF", "O"/"C" (open/close),
                // but these seem to have no effect - it is always a toggle.
                // So we are just keeping State as an integer for now.
                p.WriteString(State.ToString());
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }
    }
}
