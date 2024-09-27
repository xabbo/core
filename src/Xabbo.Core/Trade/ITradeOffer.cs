using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Represents a user's offer in a trade.
/// </summary>
public interface ITradeOffer
{
    /// <summary>
    /// The ID of the user.
    /// </summary>
    /// <remarks>
    /// Unavailable on Shockwave.
    /// </remarks>
    Id UserId { get; }

    /// <summary>
    /// The name of the user.
    /// </summary>
    /// <remarks>
    /// Only available on Shockwave.
    /// </remarks>
    string? UserName { get; }

    /// <summary>
    /// The list of items in the trade offer.
    /// </summary>
    IReadOnlyList<ITradeItem> Items { get; }

    /// <summary>
    /// The number of furni in the trade offer.
    /// </summary>
    int FurniCount { get; }

    /// <summary>
    /// The number of credits in the trade offer.
    /// </summary>
    int CreditCount { get; }
}
