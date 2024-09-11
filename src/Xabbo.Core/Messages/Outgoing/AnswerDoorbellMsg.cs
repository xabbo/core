using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

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