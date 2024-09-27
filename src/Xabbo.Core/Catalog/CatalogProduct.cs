using System;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="ICatalogProduct"/>
public sealed class CatalogProduct : ICatalogProduct, IParserComposer<CatalogProduct>
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

    string? IItem.Identifier => null;

    public CatalogProduct()
    {
        Variant = "";
    }

    private CatalogProduct(in PacketReader p)
    {
        Type = p.Client switch
        {
            ClientType.Flash => H.ToItemType(p.ReadString()),
            ClientType.Unity => H.ToItemType(p.ReadShort()),
            _ => throw new Exception($"Unknown client protocol: {p.Client}.")
        };

        if (Type == ItemType.Badge)
        {
            Variant = p.ReadString();
            Count = 1;
        }
        else
        {
            Kind = p.ReadInt();
            Variant = p.ReadString();
            Count = p.ReadInt();
            IsLimited = p.ReadBool();
            if (IsLimited)
            {
                LimitedTotal = p.ReadInt();
                LimitedRemaining = p.ReadInt();
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

    void IComposer.Compose(in PacketWriter p)
    {
        switch (p.Client)
        {
            case ClientType.Flash: p.WriteString(Type.GetClientIdentifier()); break;
            case ClientType.Unity: p.WriteShort(Type.GetClientValue()); break;
            default: throw new Exception($"Unknown client protocol: {p.Client}.");
        }

        if (Type == ItemType.Badge)
        {
            p.WriteString(Variant);
        }
        else
        {
            p.WriteInt(Kind);
            p.WriteString(Variant);
            p.WriteInt(Count);
            p.WriteBool(IsLimited);

            if (IsLimited)
            {
                p.WriteInt(LimitedTotal);
                p.WriteInt(LimitedRemaining);
            }
        }
    }

    static CatalogProduct IParser<CatalogProduct>.Parse(in PacketReader p) => new(in p);
}
