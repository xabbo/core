using System;

namespace Xabbo.Core.Events
{
    public class TradeUpdateEventArgs : EventArgs
    {
        public TradeUpdate Update { get; }

        public TradeUpdateEventArgs(TradeUpdate update)
        {
            Update = update;
        }
    }
}
