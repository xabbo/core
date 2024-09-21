using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing.Modern;

/// <summary>
/// Sent when requesting a catalog page on modern clients.
/// </summary>
public sealed record GetCatalogPageMsg(int Id, int OfferId = -1, string Type = "NORMAL") : IMessage<GetCatalogPageMsg>
{
    static bool IMessage<GetCatalogPageMsg>.UseTargetedIdentifiers => true;
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
