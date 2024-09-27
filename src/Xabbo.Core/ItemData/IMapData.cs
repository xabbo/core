using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Represents a map of string keys and values.
/// </summary>
public interface IMapData : IItemData, IReadOnlyDictionary<string, string> { }
