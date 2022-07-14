using System;
using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Defines an integer array as extra data in an item.
/// </summary>
public interface IIntArrayData : IItemData, IReadOnlyList<int> { }
