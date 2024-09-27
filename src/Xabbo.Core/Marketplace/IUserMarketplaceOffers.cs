using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Contains the user's own marketplace listings.
/// </summary>
public interface IUserMarketplaceOffers : IReadOnlyCollection<IMarketplaceOffer>
{
    /// <summary>
    /// Gets the amount of credits waiting to be claimed.
    /// </summary>
    int CreditsWaiting { get; }
}
