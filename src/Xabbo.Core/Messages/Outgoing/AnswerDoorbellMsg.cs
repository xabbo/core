using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when responding to the doorbell.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>.
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
