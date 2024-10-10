using System.Text.Json.Serialization;
using Xabbo.Core.GameData.Json;

namespace Xabbo.Core.Serialization;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    Converters = [typeof(StringConverter)]
)]
[JsonSerializable(typeof(GameDataHashesContainer))]
[JsonSerializable(typeof(GameDataHash))]
[JsonSerializable(typeof(FurniData))]
[JsonSerializable(typeof(ProductData))]
internal partial class GameDataJsonContext : JsonSerializerContext;