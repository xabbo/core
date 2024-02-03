
using System;
using System.Collections.Generic;

namespace Xabbo.Core.Events;

public class WiredMovementsEventArgs : EventArgs
{
    public IReadOnlyCollection<WiredMovement> Movements { get; }

    public WiredMovementsEventArgs(IEnumerable<WiredMovement> movements)
    {
        Movements = new List<WiredMovement>(movements).AsReadOnly();
    }
}
