using System;

namespace Xabbo.Core.Events
{
    public class TradeStopEventArgs : EventArgs
    {
        public int Reason { get; }

        public TradeStopEventArgs(int reason)
        {
            Reason = reason;
        }
    }
}
