using System;

using Xabbo.Messages;

namespace Xabbo.Core;

public class CatalogPageItem : ICatalogPageItem
{
    public int Position { get; set; }
    public string Name { get; set; }
    public string PromotionalImage { get; set; }
    public int Type { get; set; }
    public string Location { get; set; }
    public int ProductOfferId { get; set; }
    public string ProductCode { get; set; }
    public int SecondsToExpiration { get; set; }

    public CatalogPageItem()
    {
        Name =
        PromotionalImage =
        Location =
        ProductCode = string.Empty;
    }

    protected CatalogPageItem(IReadOnlyPacket packet)
        : this()
    {
        Position = packet.ReadInt();
        Name = packet.ReadString();
        PromotionalImage = packet.ReadString();
        Type = packet.ReadInt();

        switch (Type)
        {
            case 0:
                Location = packet.ReadString();
                break;
            case 1:
                ProductOfferId = packet.ReadInt();
                break;
            case 2:
                ProductCode = packet.ReadString();
                break;
            default:
                break;
        }

        SecondsToExpiration = packet.ReadInt();
    }

    public void Compose(IPacket packet)
    {
        packet
            .WriteInt(Position)
            .WriteString(Name)
            .WriteString(PromotionalImage)
            .WriteInt(Type);

        switch (Type)
        {
            case 0:
                packet.WriteString(Location);
                break;
            case 1:
                packet.WriteInt(ProductOfferId);
                break;
            case 2:
                packet.WriteString(ProductCode);
                break;
            default:
                break;
        }

        packet.WriteInt(SecondsToExpiration);
    }

    public static CatalogPageItem Parse(IReadOnlyPacket packet) => new CatalogPageItem(packet);
}
