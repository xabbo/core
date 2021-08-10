using System;
using System.Collections.Generic;

namespace Xabbo.Core
{
    /// <summary>
    /// Defines a string to string map as extra data in an item.
    /// </summary>
    public interface IMapData : IItemData, IReadOnlyDictionary<string, string> { }
}
