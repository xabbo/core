using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when requesting a catalog page.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.GetCatalogPage"/></item>
/// </list>
/// </summary>
public sealed record GetCatalogPageMsg(int Id, int OfferId = -1, string Type = "NORMAL") : IMessage<GetCatalogPageMsg>
{
    static ClientType IMessage<GetCatalogPageMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<GetCatalogPageMsg>.Identifier => Out.GetCatalogPage;

    static GetCatalogPageMsg IParser<GetCatalogPageMsg>.Parse(in PacketReader p) => new(
        Id: p.ReadInt(),
        OfferId: p.ReadInt(),
        Type: p.ReadString()
    );

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Id);
        p.WriteInt(OfferId);
        p.WriteString(Type);
    }
}
