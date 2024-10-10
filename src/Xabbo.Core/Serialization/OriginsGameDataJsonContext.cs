using System.Text.Json.Serialization;
using Xabbo.Core.GameData.Json.Origins;

namespace Xabbo.Core.Serialization;

[JsonSourceGenerationOptions(
    NumberHandling = JsonNumberHandling.AllowReadingFromString
)]
[JsonSerializable(typeof(FigureData))]
[JsonSerializable(typeof(FurniData))]
internal partial class OriginsGameDataJsonContext : JsonSerializerContext;