using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when shouting in a room.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.Shout"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.SHOUT"/></item>
/// </list>
/// </summary>
/// <param name="Message">The chat message content.</param>
/// <param name="BubbleStyle">The chat bubble style. Applies to <see cref="ClientType.Modern"/> clients.</param>
public sealed record ShoutMsg(string Message, int BubbleStyle = 0) : IMessage<ShoutMsg>
{
    static Identifier IMessage<ShoutMsg>.Identifier => Out.Shout;

    static ShoutMsg IParser<ShoutMsg>.Parse(in PacketReader p) => new(
        p.ReadString(),
        p.Client is ClientType.Shockwave ? 0 : p.ReadInt()
    );

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteString(Message);
        if (p.Client is not ClientType.Shockwave)
            p.WriteInt(BubbleStyle);
    }
}
