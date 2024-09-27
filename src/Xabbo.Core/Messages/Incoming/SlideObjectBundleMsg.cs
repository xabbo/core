using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when a group of items and/or an avatar are moved by a roller.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>.
/// </summary>
/// <param name="Bundle">The group of items being updated.</param>
public sealed record SlideObjectBundleMsg(SlideObjectBundle Bundle) : IMessage<SlideObjectBundleMsg>
{
    static Identifier IMessage<SlideObjectBundleMsg>.Identifier => In.SlideObjectBundle;
    static SlideObjectBundleMsg IParser<SlideObjectBundleMsg>.Parse(in PacketReader p) => new(p.Parse<SlideObjectBundle>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Bundle);
}
