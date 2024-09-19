using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when requesting the user's credit balance.
/// </summary>
public sealed record GetCreditsMsg : IMessage<GetCreditsMsg>
{
    static Identifier IMessage<GetCreditsMsg>.Identifier => Out.GetCreditsInfo;
    static GetCreditsMsg IParser<GetCreditsMsg>.Parse(in PacketReader p) => new();
    void IComposer.Compose(in PacketWriter p) { }
}
