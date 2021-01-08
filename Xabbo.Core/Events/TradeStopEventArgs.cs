using System;

namespace Xabbo.Core.Events
{
    public class TradeStopEventArgs : EventArgs
    {
        public IRoomUser User { get; }
        public int Reason { get; }

        public TradeStopEventArgs(IRoomUser user, int reason)
        {
            User = user;
            Reason = reason;
        }
    }
}
