using System;

namespace Xabbo.Core.Events
{
    public class TradeAcceptEventArgs : EventArgs
    {
        public IRoomUser User { get; }
        public bool Accepted { get; }

        public TradeAcceptEventArgs(IRoomUser user, bool accepted)
        {
            User = user;
            Accepted = accepted;
        }
    }
}
