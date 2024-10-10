using System.Text.Json.Serialization;

using Xabbo.Core.GameData;

namespace Xabbo.Core.Serialization;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    Converters = [typeof(StringConverter)]
)]
[JsonSerializable(typeof(Hotel))]
[JsonSerializable(typeof(GameDataHashes))]
internal partial class XabboJsonContext : JsonSerializerContext;