using System.Collections.Generic;

namespace Xabbo.Core;

public interface ITradeOffer
{
    /// <summary>
    /// Gets the user's ID (on modern clients).
    /// </summary>
    Id UserId { get; }
    /// <summary>
    /// Gets the user's name (on the Origins client).
    /// </summary>
    string? UserName { get; }
    IReadOnlyList<ITradeItem> Items { get; }
    int FurniCount { get; }
    int CreditCount { get; }
}
