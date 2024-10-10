namespace Xabbo.Core.GameData;

/// <summary>
/// The default <see cref="IGameDataUrlProvider"/> implementation.
/// </summary>
public class GameDataUrlProvider : IGameDataUrlProvider
{
    public string? GetHashesUrl(Hotel hotel)
    {
        if (hotel.IsOrigins)
        {
            return null;
        }

        return $"https://{hotel.WebHost}/gamedata/hashes2";
    }

    public string? GetGameDataUrl(Hotel hotel, GameDataType type, string? hash = null)
    {
        hash ??= "1";

        if (hotel.IsOrigins)
        {
            string baseUrl = $"https://{hotel.WebHost}";
            string baseUrlGamedata = $"https://{hotel.Subdomain}-gamedata.habbo.{hotel.Domain}";

            return type switch
            {
                // Ones with /1 for the hash fail to fetch when using the actual hash,
                // but the MD5 hash returned in the ETag header is still used locally.
                GameDataType.FigureData => $"{baseUrlGamedata}/figuredata/1",
                GameDataType.FurniData => $"{baseUrl}/gamedata/furnidata/{hash}",
                GameDataType.ExternalTexts => $"{baseUrlGamedata}/external_texts/1",
                _ => null
            };
        }
        else
        {
            string? path = type switch
            {
                GameDataType.FigureData => "gamedata/figuredata",
                GameDataType.FurniData => "gamedata/furnidata_json",
                GameDataType.ExternalTexts => "gamedata/external_flash_texts",
                GameDataType.ExternalVariables => "gamedata/external_flash_texts",
                GameDataType.ProductData => "gamedata/productdata_json",
                _ => null,
            };

            if (path is null)
                return null;

            return $"https://{hotel.WebHost}/{path}/{hash}";
        }
    }
}