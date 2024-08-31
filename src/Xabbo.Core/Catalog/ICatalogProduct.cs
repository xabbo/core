namespace Xabbo.Core;

public interface ICatalogProduct : IItem
{
    string Variant { get; }
    int Count { get; }
    bool IsLimited { get; }
    int LimitedTotal { get; }
    int LimitedRemaining { get; }
}
