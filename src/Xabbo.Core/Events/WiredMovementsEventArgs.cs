using System.Collections.Generic;

namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.WiredMovements"/> event.
/// </summary>
/// <param name="movements">The list of wired movements.</param>
public sealed class WiredMovementsEventArgs(IEnumerable<WiredMovement> movements)
{
    /// <summary>
    /// The list of wired movements.
    /// </summary>
    public IReadOnlyCollection<WiredMovement> Movements { get; } = new List<WiredMovement>(movements).AsReadOnly();
}
