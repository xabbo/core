using System;
using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Defines a string array as extra data in an item.
/// </summary>
public interface IStringArrayData : IItemData, IReadOnlyList<string> { }
