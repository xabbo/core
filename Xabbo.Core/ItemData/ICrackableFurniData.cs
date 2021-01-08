using System;

namespace Xabbo.Core
{
    public interface ICrackableFurniData : IItemData
    {
        int Hits { get; }
        int Target { get; }
    }
}
