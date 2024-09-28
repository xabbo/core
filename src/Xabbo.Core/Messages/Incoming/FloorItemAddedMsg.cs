using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when a floor item is added to the room.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.ObjectAdd"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.ACTIVEOBJECT_ADD"/></item>
/// </list>
/// </summary>
/// <param name="Item">The floor item that was added.</param>
public sealed record FloorItemAddedMsg(FloorItem Item) : IMessage<FloorItemAddedMsg>
{
    static Identifier IMessage<FloorItemAddedMsg>.Identifier => In.ObjectAdd;
    static FloorItemAddedMsg IParser<FloorItemAddedMsg>.Parse(in PacketReader p)
    {
        var item = p.Parse<FloorItem>();
        if (p.Client is not ClientType.Shockwave)
            item.OwnerName = p.ReadString();
        return new(item);
    }
    void IComposer.Compose(in PacketWriter p)
    {
        p.Compose(Item);
        if (p.Client is not ClientType.Shockwave)
            p.WriteString(Item.OwnerName);
    }
}
