using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when requesting a catalog page.
/// <para/>
/// Supported clients: <see cref="ClientType.Origins"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.GET_CATALOG_PAGE"/></item>
/// </list>
/// </summary>
public sealed record GetCatalogPageOriginsMsg(string Name, string Type = "production", string Locale = "en") : IMessage<GetCatalogPageOriginsMsg>
{
    static ClientType IMessage<GetCatalogPageOriginsMsg>.SupportedClients => ClientType.Origins;
    static Identifier IMessage<GetCatalogPageOriginsMsg>.Identifier => Out.GetCatalogPage;

    static GetCatalogPageOriginsMsg IParser<GetCatalogPageOriginsMsg>.Parse(in PacketReader p) => new(
        Type: p.ReadString(),
        Name: p.ReadString(),
        Locale: p.ReadString()
    );

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteString(Type);
        p.WriteString(Name);
        p.WriteString(Locale);
    }
}
