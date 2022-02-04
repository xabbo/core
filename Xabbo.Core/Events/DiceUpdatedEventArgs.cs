using System;

namespace Xabbo.Core.Events
{
    public class DiceUpdatedEventArgs : FloorItemEventArgs
    {
        /// <summary>
        /// Gets the previous value of the dice.
        /// </summary>
        public int PreviousValue { get; }
        /// <summary>
        /// Gets the current value of the dice.
        /// </summary>
        public int CurrentValue { get; }

        public DiceUpdatedEventArgs(IFloorItem item, int previousValue)
            : base(item)
        {
            PreviousValue = previousValue;
            CurrentValue = item.Data.State;
        }
    }
}
