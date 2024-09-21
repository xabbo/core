using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing.Origins;

/// <summary>
/// Sent when requesting a catalog page on Origins.
/// </summary>
public sealed record GetCatalogPageMsg(string Name, string Type = "production", string Locale = "en") : IMessage<GetCatalogPageMsg>
{
    static Identifier IMessage<GetCatalogPageMsg>.Identifier => Out.GetCatalogPage;

    static GetCatalogPageMsg IParser<GetCatalogPageMsg>.Parse(in PacketReader p) => new(
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
