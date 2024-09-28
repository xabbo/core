using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when an avatar's hand item updates.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.CarryObject"/></item>
/// </list>
/// </summary>
/// <param name="Index">The index of the avatar.</param>
/// <param name="Item">The avatar's updated hand item ID.</param>
public sealed record AvatarHandItemMsg(int Index, int Item) : IMessage<AvatarHandItemMsg>
{
    static ClientType IMessage<AvatarHandItemMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<AvatarHandItemMsg>.Identifier => In.CarryObject;

    static AvatarHandItemMsg IParser<AvatarHandItemMsg>.Parse(in PacketReader p) => new(p.ReadInt(), p.ReadInt());
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Index);
        p.WriteInt(Item);
    }
}
