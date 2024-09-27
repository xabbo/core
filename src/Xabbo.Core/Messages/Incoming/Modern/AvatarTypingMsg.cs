using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when an avatar in the room starts or stops typing.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>.
/// </summary>
/// <param name="Index">The index of the avatar.</param>
/// <param name="Typing">Whether the avatar is typing.</param>
public sealed record AvatarTypingMsg(int Index, bool Typing) : IMessage<AvatarTypingMsg>
{
    static ClientType IMessage<AvatarTypingMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<AvatarTypingMsg>.Identifier => In.UserTyping;

    static AvatarTypingMsg IParser<AvatarTypingMsg>.Parse(in PacketReader p) => new(p.ReadInt(), p.ReadInt() != 0);
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Index);
        p.WriteInt(Typing ? 1 : 0);
    }
}
