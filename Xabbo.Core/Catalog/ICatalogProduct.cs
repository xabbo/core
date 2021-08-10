using System;

using Xabbo.Messages; 

namespace Xabbo.Core
{
    public interface ICatalogProduct : IItem, IComposable
    {
        string Variant { get; }
        int Count { get; }
        bool IsLimited { get; }
        int LimitedTotal { get; }
        int LimitedRemaining { get; }
    }
}
