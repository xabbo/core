using System;

using Xabbo.Messages;

namespace Xabbo.Core;

public interface ICatalogPageItem : IComposer
{
    int Position { get; }
    int Type { get; }
    int SecondsToExpiration { get; set; }
}
