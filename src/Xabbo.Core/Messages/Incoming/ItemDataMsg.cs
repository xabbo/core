using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received after requesting the data of a wall item.
/// <para/>
/// Response for <see cref="Outgoing.GetItemDataMsg"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.ItemDataUpdate"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.IDATA"/></item>
/// </list>
/// </summary>
public sealed record ItemDataMsg(Id Id, string Data) : IMessage<ItemDataMsg>
{
    static Identifier IMessage<ItemDataMsg>.Identifier => In.ItemDataUpdate;

    static ItemDataMsg IParser<ItemDataMsg>.Parse(in PacketReader p) => new(
        (Id)p.ReadString(),
        p.ReadString()
    );

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteString(Id.ToString());
        p.WriteString(Data);
    }
}
