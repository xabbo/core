using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Receives when a user rings the doorbell.
/// </summary>
/// <param name="Name">The name of the user who is ringing the doorbell.</param>
public sealed record DoorbellMsg(string Name) : IMessage<DoorbellMsg>
{
    static Identifier IMessage<DoorbellMsg>.Identifier => In.Doorbell;
    static DoorbellMsg IParser<DoorbellMsg>.Parse(in PacketReader p) => new(p.ReadString());
    void IComposer.Compose(in PacketWriter p) => p.WriteString(Name);
}