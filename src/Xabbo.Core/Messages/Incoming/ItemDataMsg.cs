using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received in response to <see cref="Outgoing.GetItemDataMsg"/>
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
