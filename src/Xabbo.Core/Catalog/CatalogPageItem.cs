using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="ICatalogPageItem"/>
public sealed class CatalogPageItem : ICatalogPageItem, IParserComposer<CatalogPageItem>
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
        ProductCode = "";
    }

    private CatalogPageItem(in PacketReader p) : this()
    {
        Position = p.ReadInt();
        Name = p.ReadString();
        PromotionalImage = p.ReadString();
        Type = p.ReadInt();

        switch (Type)
        {
            case 0:
                Location = p.ReadString();
                break;
            case 1:
                ProductOfferId = p.ReadInt();
                break;
            case 2:
                ProductCode = p.ReadString();
                break;
            default:
                break;
        }

        SecondsToExpiration = p.ReadInt();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Position);
        p.WriteString(Name);
        p.WriteString(PromotionalImage);
        p.WriteInt(Type);

        switch (Type)
        {
            case 0:
                p.WriteString(Location);
                break;
            case 1:
                p.WriteInt(ProductOfferId);
                break;
            case 2:
                p.WriteString(ProductCode);
                break;
            default:
                break;
        }

        p.WriteInt(SecondsToExpiration);
    }

    static CatalogPageItem IParser<CatalogPageItem>.Parse(in PacketReader p) => new(in p);
}
