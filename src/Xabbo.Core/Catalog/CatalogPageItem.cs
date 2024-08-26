using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class CatalogPageItem : ICatalogPageItem, IComposer, IParser<CatalogPageItem>
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

    private CatalogPageItem(in PacketReader p) : this()
    {
        Position = p.Read<int>();
        Name = p.Read<string>();
        PromotionalImage = p.Read<string>();
        Type = p.Read<int>();

        switch (Type)
        {
            case 0:
                Location = p.Read<string>();
                break;
            case 1:
                ProductOfferId = p.Read<int>();
                break;
            case 2:
                ProductCode = p.Read<string>();
                break;
            default:
                break;
        }

        SecondsToExpiration = p.Read<int>();
    }

    public void Compose(in PacketWriter p)
    {
        p.Write(Position);
        p.Write(Name);
        p.Write(PromotionalImage);
        p.Write(Type);

        switch (Type)
        {
            case 0:
                p.Write(Location);
                break;
            case 1:
                p.Write(ProductOfferId);
                break;
            case 2:
                p.Write(ProductCode);
                break;
            default:
                break;
        }

        p.Write(SecondsToExpiration);
    }

    public static CatalogPageItem Parse(in PacketReader p) => new(in p);
}
