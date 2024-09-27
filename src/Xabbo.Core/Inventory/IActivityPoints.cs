using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Represents a mapping of <see cref="ActivityPointType"/> to their current amount.
/// </summary>
public interface IActivityPoints : IReadOnlyDictionary<ActivityPointType, int>;
