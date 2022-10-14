using System;

using Xabbo.Messages;

namespace Xabbo.Core;

public interface ICatalogPageItem : IComposable
{
    int Position { get; }
    int Type { get; }
    int SecondsToExpiration { get; set; }
}
