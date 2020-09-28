using System;

namespace Xabbo.Core.Events
{
    public class TradeStartEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the user who started the trade.
        /// </summary>
        public IRoomUser Trader { get; }

        /// <summary>
        /// Gets the user who was traded.
        /// </summary>
        public IRoomUser Tradee { get; }

        public TradeStartEventArgs(IRoomUser trader, IRoomUser tradee)
        {
            Trader = trader;
            Tradee = tradee;
        }
    }
}
