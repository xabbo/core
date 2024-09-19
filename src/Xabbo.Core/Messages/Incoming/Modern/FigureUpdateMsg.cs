using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

public sealed record FigureUpdateMsg(string Figure, Gender Gender) : IMessage<FigureUpdateMsg>
{
    static bool IMessage<FigureUpdateMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<FigureUpdateMsg>.Identifier => In.FigureUpdate;

    static FigureUpdateMsg IParser<FigureUpdateMsg>.Parse(in PacketReader p) => new(
        Figure: p.ReadString(),
        Gender: H.ToGender(p.ReadString())
    );

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteString(Figure);
        p.WriteString(Gender.ToClientString());
    }
}
