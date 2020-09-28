using System;

namespace Xabbo.Core.Events
{
    public class PointsUpdatedEventArgs : EventArgs
    {
        public ActivityPointType Type { get; }
        public int Amount { get; }
        public int Change { get; }

        public PointsUpdatedEventArgs(ActivityPointType type, int amount, int change)
        {
            Type = type;
            Amount = amount;
            Change = change;
        }
    }
}
