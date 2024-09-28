using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when responding to the doorbell.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.LetUserIn"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.LETUSERIN"/></item>
/// </list>
/// </summary>
/// <param name="Name">The name of the user.</param>
/// <param name="Accept">Whether to accept the user into the room.</param>
public sealed record AnswerDoorbellMsg(string Name, bool Accept) : IMessage<AnswerDoorbellMsg>
{
    static Identifier IMessage<AnswerDoorbellMsg>.Identifier => Out.LetUserIn;
    static AnswerDoorbellMsg IParser<AnswerDoorbellMsg>.Parse(in PacketReader p) => new(p.ReadString(), p.ReadBool());
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteString(Name);
        p.WriteBool(Accept);
    }
}
