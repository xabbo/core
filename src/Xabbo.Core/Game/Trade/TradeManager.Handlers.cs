using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Game;

[Intercept]
partial class TradeManager
{
    [Intercept]
    private void HandleTradeOpened(Intercept<TradeOpenedMsg> e)
    {
        using (_logger.MethodScope())
            TryStartTradeModern(e.Msg.TraderId, e.Msg.TradeeId);
    }

    [Intercept]
    private void HandleTradeOpenFailed(Intercept<TradeOpenFailedMsg> e)
    {
        using (_logger.MethodScope())
            TradeOpenFailed(e.Msg.Reason, e.Msg.UserName);
    }

    [Intercept]
    private void HandleTradingItemList(Intercept<TradeOffersMsg> e)
    {
        using (_logger.MethodScope())
            UpdateTrade(e.Msg.First, e.Msg.Second);
    }

    [Intercept]
    private void HandleTradeAccepted(Intercept<TradeAcceptedMsg> e)
    {
        using (_logger.MethodScope())
            UserAccepted(e.Msg.UserId, e.Msg.UserName, e.Msg.Accepted);
    }

    [Intercept]
    private void HandleTradingConfirmation(Intercept<TradeAwaitingConfirmationMsg> e)
    {
        using (_logger.MethodScope())
            SetAwaitingConfirmation();
    }

    [Intercept]
    void HandleTradingCompleted(Intercept<TradeCompletedMsg> e)
    {
        using (_logger.MethodScope())
            CompleteTrade();
    }

    [Intercept]
    void HandleTradingClose(Intercept<TradeClosedMsg> e)
    {
        using (_logger.MethodScope())
            CloseTrade(e.Msg.UserId, e.Msg.Reason);
    }
}