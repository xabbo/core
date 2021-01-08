using System;
using System.Collections.Generic;

namespace Xabbo.Core
{
    public interface IMapData : IItemData, IReadOnlyDictionary<string, string> { }
}
