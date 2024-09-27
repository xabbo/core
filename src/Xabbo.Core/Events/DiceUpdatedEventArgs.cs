namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.DiceUpdated"/> event.
/// </summary>
/// <param name="dice">The dice.</param>
/// <param name="previousValue">The previous value of the dice.</param>
/// <param name="updatedValue">The updated value of the dice.</param>
public sealed class DiceUpdatedEventArgs(IFloorItem dice, int previousValue, int updatedValue)
    : FloorItemEventArgs(dice)
{
    /// <summary>
    /// Gets the previous value of the dice.
    /// </summary>
    public int PreviousValue { get; } = previousValue;

    /// <summary>
    /// Gets the updated value of the dice.
    /// </summary>
    public int Value { get; } = updatedValue;
}
