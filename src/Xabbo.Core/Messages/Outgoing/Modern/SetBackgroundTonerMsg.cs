using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing.Modern;

public sealed record SetBackgroundTonerColor(Id Id, int Hue, int Saturation, int Value) : IMessage<SetBackgroundTonerColor>
{
    static ClientType IMessage<SetBackgroundTonerColor>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<SetBackgroundTonerColor>.Identifier => Out.SetRoomBackgroundColorData;

    static SetBackgroundTonerColor IParser<SetBackgroundTonerColor>.Parse(in PacketReader p)
    {
        throw new System.NotImplementedException();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        throw new System.NotImplementedException();
    }
}