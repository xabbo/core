using System;
 
namespace Xabbo.Core
{
    public interface ICatalogProduct
    {
        string Type { get; }
        int Kind { get; }
        string Variation { get; }
        int Count { get; }
        bool IsLimited { get; }
        int LimitedTotal { get; }
        int LimitedRemaining { get; }
    }
}
