using System;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xabbo.Extension;
using Xabbo.Core.Events;

namespace Xabbo.Core.Game;

/// <summary>
/// Manages the user's trading state.
/// </summary>
public sealed partial class TradeManager(
    IExtension extension,
    ProfileManager profileManager,
    RoomManager roomManager,
    ILoggerFactory? loggerFactory = null
)
    : GameStateManager(extension)
{
    private readonly ILogger _logger = (ILogger?)loggerFactory?.CreateLogger<TradeManager>() ?? NullLogger.Instance;

    private readonly ProfileManager _profileManager = profileManager;
    private readonly RoomManager _roomManager = roomManager;

    /// <summary>
    /// Gets whether the user is currently trading.
    /// </summary>
    public bool IsTrading { get; private set; }

    /// <summary>
    /// Gets whether the user initiated the current trade.
    /// </summary>
    public bool IsTrader { get; private set; }

    /// <summary>
    /// Gets whether the trade has completed.
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Gets the user's own room instance.
    /// </summary>
    public IUser? Self { get; private set; }

    /// <summary>
    /// Gets the trading parter's room user instance.
    /// </summary>
    public IUser? Partner { get; private set; }

    /// <summary>
    /// Gets the user's own trade offer.
    /// </summary>
    public ITradeOffer? SelfOffer { get; private set; }

    /// <summary>
    /// Gets the trading partner's offer.
    /// </summary>
    public ITradeOffer? PartnerOffer { get; private set; }

    /// <summary>
    /// Gets whether the user has accepted the current trade.
    /// </summary>
    public bool HasAccepted { get; private set; }

    /// <summary>
    /// Gets whether the trading partner has accepted the current trade.
    /// </summary>
    public bool HasPartnerAccepted { get; private set; }

    /// <summary>
    /// Gets whether the trade is awaiting confirmation of both users.
    /// </summary>
    /// <remarks>
    /// Used on modern clients.
    /// </remarks>
    public bool IsAwaitingConfirmation { get; private set; }

    private bool TryStartTradeOrigins(IRoom room, TradeOffer traderOffer, TradeOffer tradeeOffer)
    {
        if (traderOffer.Count > 0 || tradeeOffer.Count > 0)
            return false;

        if (traderOffer is not { UserName: string traderName } ||
            tradeeOffer is not { UserName: string tradeeName })
        {
            _logger.LogWarning("Trader names unavailable.");
            return false;
        }

        if (!room.TryGetUserByName(traderName, out var trader))
        {
            _logger.LogWarning("Failed to find user with name '{UserName}'.", traderName);
            return false;
        }

        if (!room.TryGetUserByName(tradeeName, out var tradee))
        {
            _logger.LogWarning("Failed to find user with name '{UserName}'.", tradeeName);
            return false;
        }

        return TryStartTrade(trader, tradee);
    }

    private bool TryStartTradeModern(Id traderId, Id tradeeId)
    {
        if (!_roomManager.IsInRoom || _roomManager.Room is null)
        {
            _logger.LogDebug("User is not in a room.");
            return false;
        }

        if (IsTrading)
        {
            _logger.LogWarning("The user is currently trading.");
            return false;
        }

        if (!_roomManager.Room.TryGetAvatarById(traderId, out IUser? trader))
        {
            _logger.LogWarning("Failed to find user with ID {Id}.", traderId);
            return false;
        }

        if (!_roomManager.Room.TryGetAvatarById(tradeeId, out IUser? tradee))
        {
            _logger.LogWarning("Failed to find user with ID {Id}.", tradeeId);
            return false;
        }

        return TryStartTrade(trader, tradee);
    }

    private bool TryStartTrade(IUser trader, IUser tradee)
    {
        ResetTrade();

        if (_profileManager.UserData is not { Id: Id selfId, Name: string selfName })
        {
            _logger.LogWarning("User data is not available.");
            return false;
        }

        if (Session.Is(ClientType.Origins))
            IsTrader = trader.Name.Equals(selfName);
        else
            IsTrader =  trader.Id == selfId;
        Self = IsTrader ? trader : tradee;
        Partner = IsTrader ? tradee : trader;

        IsTrading = true;

        _logger.LogInformation("Trade opened with {UserName}.", Partner.Name);
        Opened?.Invoke(new TradeOpenedEventArgs(IsTrader, Self, Partner));

        return true;
    }

    private void UpdateTrade(TradeOffer first, TradeOffer second)
    {
        if (!_roomManager.EnsureInRoom(out var room))
        {
            _logger.LogDebug("Not in a room.");
            return;
        }

        if (!IsTrading)
        {
            // Origins does not have a trade open packet.
            // The trade begins with an empty trading list.
            if (Session.Is(ClientType.Origins))
            {
                if (!TryStartTradeOrigins(room, first, second))
                    return;
            }
            else
            {
                _logger.LogDebug("User is not trading.");
                return;
            }
        }

        HasAccepted =
        HasPartnerAccepted = false;

        // Shockwave always has the trader's offer first.
        if (Session.Is(ClientType.Origins))
        {
            SelfOffer = IsTrader ? first : second;
            PartnerOffer = IsTrader ? second : first;
        }
        else
        // Modern clients always have your own offer first.
        {
            SelfOffer = first;
            PartnerOffer = second;
        }

        _logger.LogDebug("Trade updated.");
        Updated?.Invoke(new TradeUpdatedEventArgs(SelfOffer, PartnerOffer));
    }

    private void UserAccepted(Id? userId, string? userName, bool accepted)
    {
        if (!_roomManager.IsInRoom)
        {
            _logger.LogDebug("Not in a room.");
            return;
        }

        if (!IsTrading)
        {
            _logger.LogDebug("User is not trading.");
            return;
        }

        IUser? user;

        if (Self is not { } self || Partner is not { } partner)
            return;

        if (userId == self.Id || userName == self.Name)
        {
            user = Self;
            HasAccepted = accepted;
        }
        else if (userId == partner.Id || userName == partner.Name)
        {
            user = Partner;
            HasPartnerAccepted = accepted;
        }
        else
        {
            _logger.LogWarning("User ID or name does not match self or trading partner.");
            return;
        }

        _logger.LogInformation("User '{UserName}' accepted: {Accepted}", user.Name, accepted);
        Accepted?.Invoke(new TradeAcceptedEventArgs(user, accepted));
    }

    private void SetAwaitingConfirmation()
    {
        if (!_roomManager.IsInRoom)
        {
            _logger.LogDebug("Not in a room.");
            return;
        }

        if (!IsTrading)
        {
            _logger.LogDebug("User is not trading.");
            return;
        }

        IsAwaitingConfirmation = true;
        AwaitingConfirmation?.Invoke();
    }

    private void CompleteTrade()
    {
        if (!IsTrading) return;

        if (IsCompleted)
        {
            _logger.LogWarning("Trade already completed!");
            return;
        }

        bool wasTrader = IsTrader;

        if (Self is not { } self)
        {
            _logger.LogWarning("Self is null.");
            return;
        }

        if (Partner is not { } partner)
        {
            _logger.LogWarning("Partner is null.");
            return;
        }

        if (SelfOffer is not { } selfOffer)
        {
            _logger.LogWarning("Self offer is null.");
            return;
        }

        if (PartnerOffer is not { } partnerOffer)
        {
            _logger.LogWarning("Partner offer is null");
            return;
        }

        IsCompleted = true;

        _logger.LogInformation("Trade completed with '{PartnerName}'.", partner.Name);
        Completed?.Invoke(new TradeCompletedEventArgs(wasTrader, self, partner, selfOffer, partnerOffer));
    }

    private void CloseTrade(Id? userId, int? reason)
    {
        if (!IsTrading) return;

        IUser? closer = null;

        try
        {
            if (userId is not null && reason.HasValue)
            {
                if (Self is not { } self)
                {
                    _logger.LogWarning("Self is null.");
                    return;
                }

                if (Partner is not { } partner)
                {
                    _logger.LogWarning("Partner is null.");
                    return;
                }

                if (userId == self.Id) closer = self;
                else if (userId == partner.Id) closer = partner;

                if (closer is null)
                {
                    _logger.LogWarning(
                        "User ID mismatch: #{UserId} was not self (#{SelfId}) or partner (#{PartnerId}).",
                        userId, self.Id, partner.Id);
                    return;
                }

                _logger.LogInformation("Trade closed by '{UserName}', reason: {Reason}.", closer.Name, reason);
            }
        }
        finally
        {
            Closed?.Invoke(new TradeClosedEventArgs(closer, reason));
            ResetTrade();
        }
    }

    private void TradeOpenFailed(int reason, string name)
    {
        if (!_roomManager.IsInRoom)
        {
            _logger.LogDebug("Not in a room.");
            return;
        }

        OpenFailed?.Invoke(new TradeOpenFailedEventArgs(reason, name));
    }

    protected override void OnDisconnected() => ResetTrade();

    private void ResetTrade()
    {
        IsTrading =
        IsTrader =
        HasAccepted =
        HasPartnerAccepted =
        IsAwaitingConfirmation =
        IsCompleted = false;

        Self =
        Partner = null;

        SelfOffer =
        PartnerOffer = null;
    }
}
