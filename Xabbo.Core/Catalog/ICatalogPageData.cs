using System;

namespace Xabbo.Core
{
    public interface ICatalogPageData
    {
        int Position { get; }
        int Type { get; }
        int SecondsToExpiration { get; set; }
    }
}
