using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Receives when a user rings the doorbell.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.Doorbell"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.DOORBELL_RINGING"/></item>
/// </list>
/// </summary>
/// <param name="Name">The name of the user who is ringing the doorbell.</param>
public sealed record DoorbellMsg(string Name) : IMessage<DoorbellMsg>
{
    static Identifier IMessage<DoorbellMsg>.Identifier => In.Doorbell;
    static DoorbellMsg IParser<DoorbellMsg>.Parse(in PacketReader p) => new(p.ReadString());
    void IComposer.Compose(in PacketWriter p) => p.WriteString(Name);
}