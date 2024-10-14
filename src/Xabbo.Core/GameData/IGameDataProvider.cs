namespace Xabbo.Core.GameData;

public interface IGameDataProvider
{
    /// <summary>
    /// Gets the figure data, if it is available.
    /// </summary>
    FigureData? Figure { get; }

    /// <summary>
    /// Gets the furni data, if it is available.
    /// </summary>
    FurniData? Furni { get; }

    /// <summary>
    /// Gets the product data, if it is available.
    /// </summary>
    ProductData? Products { get; }

    /// <summary>
    /// Gets the external texts, if they are available.
    /// </summary>
    ExternalTexts? Texts { get; }
}