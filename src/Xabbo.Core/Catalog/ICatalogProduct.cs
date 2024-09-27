namespace Xabbo.Core;

/// <summary>
/// Represents a product in a catalog offer.
/// </summary>
public interface ICatalogProduct : IItem
{
    string Variant { get; }
    int Count { get; }
    bool IsLimited { get; }
    int LimitedTotal { get; }
    int LimitedRemaining { get; }
}
