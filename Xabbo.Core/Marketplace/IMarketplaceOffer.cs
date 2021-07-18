using System;
using Xabbo.Messages;

namespace Xabbo.Core
{
    public interface IMarketplaceOffer : IItem, IComposable
    {
        /// <summary>
        /// Gets the ID of the marketplace offer.
        /// </summary>
        new long Id { get; }
        /// <summary>
        /// Gets the current status of this marketplace offer.
        /// </summary>
        MarketplaceOfferStatus Status { get; }
        /// <summary>
        /// Gets the item data for the marketplace offer.
        /// </summary>
        IItemData Data { get; }
        /// <summary>
        /// Gets the limited rare number of this item.
        /// </summary>
        int LimitedNumber { get; }
        /// <summary>
        /// Gets the total limited rares of this item.
        /// </summary>
        int LimitedTotal { get; }
        /// <summary>
        /// Gets the price of the marketplace offer.
        /// </summary>
        int Price { get; }

        int TimeRemaining { get; }
        /// <summary>
        /// Gets the average price for this item.
        /// </summary>
        int Average { get; }
        /// <summary>
        /// Gets the number of open offers for this item.
        /// Not available when loaded from the user's own marketplace offers.
        /// </summary>
        int Offers { get; }
    }
}
