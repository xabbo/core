using System.Text.Json.Serialization;

using Xabbo.Core.Web;
using Xabbo.Core.GameData.Json;
using Xabbo.Core.GameData.Json.Origins;

namespace Xabbo.Core.Serialization;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    Converters = [typeof(StringConverter)]
)]
[JsonSerializable(typeof(Hotel))]
[JsonSerializable(typeof(GameDataHashesContainer))]
[JsonSerializable(typeof(GameDataHash))]
[JsonSerializable(typeof(FurniData))]
internal partial class JsonContext : JsonSerializerContext;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
    Converters = [typeof(StringConverter)]
)]
[JsonSerializable(typeof(ProductData))]
internal partial class JsonProductContext : JsonSerializerContext;

[JsonSourceGenerationOptions(
    NumberHandling = JsonNumberHandling.AllowReadingFromString
)]
[JsonSerializable(typeof(FigureData))]
internal partial class JsonOriginsContext : JsonSerializerContext;