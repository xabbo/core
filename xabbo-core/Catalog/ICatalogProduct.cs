using System;
 
namespace Xabbo.Core
{
    public interface ICatalogProduct : IItem
    {
        string Variation { get; }
        int Count { get; }
        bool IsLimited { get; }
        int LimitedTotal { get; }
        int LimitedRemaining { get; }
    }
}
