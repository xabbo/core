using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record SlideObjectBundleMsg(SlideObjectBundle Bundle) : IMessage<SlideObjectBundleMsg>
{
    static Identifier IMessage<SlideObjectBundleMsg>.Identifier => In.SlideObjectBundle;
    static SlideObjectBundleMsg IParser<SlideObjectBundleMsg>.Parse(in PacketReader p) => new(p.Parse<SlideObjectBundle>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Bundle);
}
