using System;

namespace Xabbo.Core.Events
{
    public class TradeStartEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the user who started the trade.
        /// </summary>
        public RoomUser Trader { get; }

        /// <summary>
        /// Gets the user who was traded.
        /// </summary>
        public RoomUser Tradee { get; }

        public TradeStartEventArgs(RoomUser trader, RoomUser tradee)
        {
            Trader = trader;
            Tradee = tradee;
        }
    }
}
