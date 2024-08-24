using System.Collections.Generic;

namespace Xabbo.Core.Events;

public sealed class WiredMovementsEventArgs(IEnumerable<WiredMovement> movements)
{
    public IReadOnlyCollection<WiredMovement> Movements { get; } = new List<WiredMovement>(movements).AsReadOnly();
}
