using System;

namespace Xabbo.Core.Events
{
    public class TradeStartEventArgs : EventArgs
    {
        /// <summary>
        /// Gets if the user was the one who initiated the trade.
        /// </summary>
        public bool IsTrader { get; }

        /// <summary>
        /// Gets the partner of the trade.
        /// </summary>
        public IRoomUser Partner { get; }

        public TradeStartEventArgs(bool isTrader, IRoomUser partner)
        {
            IsTrader = isTrader;
            Partner = partner;
        }
    }
}
