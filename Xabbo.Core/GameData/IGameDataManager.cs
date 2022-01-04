using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xabbo.Core.GameData
{
    public interface IGameDataManager
    {
        /// <summary>
        /// Gets the cache path where game data is stored.
        /// </summary>
        string CachePath { get; }

        /// <summary>
        /// Gets the figure data if it is available.
        /// </summary>
        FigureData? Figure { get; }
        /// <summary>
        /// Gets the furni data if it is available.
        /// </summary>
        FurniData? Furni { get; }
        /// <summary>
        /// Gets the product data if it is available.
        /// </summary>
        ProductData? Products { get; }
        /// <summary>
        /// Gets the external texts if they are available.
        /// </summary>
        ExternalTexts? Texts { get; }

        /// <summary>
        /// Invoked when game data begins loading.
        /// </summary>
        event Action Loading;

        /// <summary>
        /// Invoked when game data is successfully loaded.
        /// </summary>
        event Action Loaded;

        /// <summary>
        /// Invoked when game data fails to load.
        /// </summary>
        event Action<Exception> LoadFailed;

        /// <summary>
        /// Loads game data for the specified domain.
        /// </summary>
        Task LoadAsync(string domain, CancellationToken cancellationToken);

        /// <summary>
        /// Waits for game data to load.
        /// </summary>
        Task WaitForLoadAsync(CancellationToken cancellationToken);
    }
}
