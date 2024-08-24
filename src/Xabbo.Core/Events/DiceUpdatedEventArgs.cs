namespace Xabbo.Core.Events;

public sealed class DiceUpdatedEventArgs(IFloorItem dice, int previous, int value)
    : FloorItemEventArgs(dice)
{
    /// <summary>
    /// The previous value of the dice.
    /// </summary>
    public int PreviousValue { get; } = previous;

    /// <summary>
    /// The current value of the dice.
    /// </summary>
    public int Value { get; } = value;
}
