using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when the user's inventory is invalidated and should be reloaded.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.FurniListAddOrUpdate"/></item>
/// </list>
/// </summary>
public sealed record InventoryInvalidatedMsg()
    : IMessage<InventoryInvalidatedMsg>
{
    static ClientType IMessage<InventoryInvalidatedMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<InventoryInvalidatedMsg>.Identifier => In.FurniListInvalidate;
    static InventoryInvalidatedMsg IParser<InventoryInvalidatedMsg>.Parse(in PacketReader p) => new();
    void IComposer.Compose(in PacketWriter p) { }
}