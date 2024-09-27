namespace Xabbo.Core;

/// <summary>
/// Represents the status of an offer in the marketplace.
/// </summary>
public enum MarketplaceOfferStatus
{
    /// <summary>
    /// The offer is currently open.
    /// </summary>
    Open = 1,
    /// <summary>
    /// The item has been sold.
    /// </summary>
    Sold = 2,
    /// <summary>
    /// The item was not sold.
    /// </summary>
    NotSold = 3
}
