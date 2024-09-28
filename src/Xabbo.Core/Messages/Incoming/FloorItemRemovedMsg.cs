using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when a floor item is removed from the room.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.ObjectRemove"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.ACTIVEOBJECT_REMOVE"/></item>
/// </list>
/// </summary>
/// <param name="Id">The ID of the floor item that was removed.</param>
/// <param name="Item">The instance of the floor item that was removed. Only available on <see cref="ClientType.Shockwave"/></param>
/// <param name="Expired">Whether the item expired or not. Applies to <see cref="ClientType.Modern"/> clients.</param>
/// <param name="PickerId">The ID of the user who picked up the item. Applies to <see cref="ClientType.Modern"/> clients.</param>
/// <param name="Delay">The delay in milliseconds after which the item will be removed by the client. Applies to <see cref="ClientType.Modern"/> clients.</param>
public sealed record FloorItemRemovedMsg(
    Id Id,
    FloorItem? Item = null,
    bool Expired = false,
    Id PickerId = default,
    int Delay = 0
)
    : IMessage<FloorItemRemovedMsg>
{
    public FloorItemRemovedMsg(FloorItem Item, bool Expired = false, Id PickerId = default, int Delay = 0)
        : this(Item.Id, Item, Expired, PickerId, Delay) { }

    static Identifier IMessage<FloorItemRemovedMsg>.Identifier => In.ObjectRemove;

    static FloorItemRemovedMsg IParser<FloorItemRemovedMsg>.Parse(in PacketReader p)
    {
        Id id;
        FloorItem? item = null;
        bool expired = false;
        Id pickerId = 0;
        int delay = 0;

        switch (p.Client)
        {
            case ClientType.Unity or ClientType.Flash:
                if (p.Client is ClientType.Unity)
                {
                    id = p.ReadId();
                }
                else
                {
                    string strId = p.ReadString();
                    if (!Id.TryParse(strId, out id))
                        throw new FormatException($"Failed to parse {nameof(Id)} in {nameof(FloorItemRemovedMsg)}: '{strId}'.");
                }
                expired = p.ReadBool();
                pickerId = p.ReadId();
                delay = p.ReadInt();
                break;
            case ClientType.Shockwave:
                item = p.Parse<FloorItem>();
                id = item.Id;
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }

        return new(id, item);
    }

    void IComposer.Compose(in PacketWriter p)
    {
        switch (p.Client)
        {
            case ClientType.Unity or ClientType.Flash:
                if (p.Client is ClientType.Unity)
                    p.WriteId(Id);
                else
                    p.WriteString(Id.ToString());
                p.WriteBool(Expired);
                p.WriteId(PickerId);
                p.WriteInt(Delay);
                break;
            case ClientType.Shockwave:
                // Shockwave sends the entire structure, but the client only cares about the ID.
                if (Item is null)
                    p.WriteString(Id.ToString());
                else
                    p.Compose(Item);
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }
    }
}
