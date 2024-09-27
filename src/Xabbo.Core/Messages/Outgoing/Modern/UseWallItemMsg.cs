using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing.Modern;

/// <summary>
/// Sent when using a wall item.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>.
/// </summary>
/// <param name="Id">The ID of the wall item.</param>
/// <param name="State">The state of the wall item. Appears to be unused - items are always toggled between states.</param>
public sealed record UseWallItemMsg(Id Id, int State = 0) : IMessage<UseWallItemMsg>
{
    static ClientType IMessage<UseWallItemMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<UseWallItemMsg>.Identifier => Out.UseWallItem;

    static UseWallItemMsg IParser<UseWallItemMsg>.Parse(in PacketReader p) => new(p.ReadId(), p.ReadInt());
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteInt(State);
    }
}