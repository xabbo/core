using System;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class CatalogProduct : ICatalogProduct, IComposer, IParser<CatalogProduct>
{
    public ItemType Type { get; set; }
    public int Kind { get; set; }
    public string Variant { get; set; }
    public int Count { get; set; }
    public bool IsLimited { get; set; }
    public int LimitedTotal { get; set; }
    public int LimitedRemaining { get; set; }

    public bool IsFloorItem => Type == ItemType.Floor;
    public bool IsWallItem => Type == ItemType.Wall;
    Id IItem.Id => -1;

    public CatalogProduct()
    {
        Variant = string.Empty;
    }

    private CatalogProduct(in PacketReader p)
    {
        Type = p.Client switch
        {
            ClientType.Flash => H.ToItemType(p.Read<string>()),
            ClientType.Unity => H.ToItemType(p.Read<short>()),
            _ => throw new Exception($"Unknown client protocol: {p.Client}.")
        };

        if (Type == ItemType.Badge)
        {
            Variant = p.Read<string>();
            Count = 1;
        }
        else
        {
            Kind = p.Read<int>();
            Variant = p.Read<string>();
            Count = p.Read<int>();
            IsLimited = p.Read<bool>();
            if (IsLimited)
            {
                LimitedTotal = p.Read<int>();
                LimitedRemaining = p.Read<int>();
            }
        }
    }

    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(Variant))
        {
            return $"{nameof(CatalogProduct)}/{Type}:{Kind}";
        }
        else
        {
            return $"{nameof(CatalogProduct)}/{Type}:{Kind}:{Variant}";
        }
    }

    public void Compose(in PacketWriter p)
    {
        switch (p.Client)
        {
            case ClientType.Flash: p.Write(Type.ToShortString()); break;
            case ClientType.Unity: p.Write(Type.GetValue()); break;
            default: throw new Exception($"Unknown client protocol: {p.Client}.");
        }

        if (Type == ItemType.Badge)
        {
            p.Write(Variant);
        }
        else
        {
            p.Write(Kind);
            p.Write(Variant);
            p.Write(Count);
            p.Write(IsLimited);

            if (IsLimited)
            {
                p.Write(LimitedTotal);
                p.Write(LimitedRemaining);
            }
        }
    }

    public static CatalogProduct Parse(in PacketReader p) => new(in p);
}
