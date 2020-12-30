using System;
using Newtonsoft.Json;

namespace Xabbo.Core.Serialization
{
    public class TileConverter : JsonConverter<Tile>
    {
        public override Tile ReadJson(JsonReader reader, Type objectType,
            Tile existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
                return Tile.Parse((string)reader.Value);

            if (reader.TokenType != JsonToken.StartArray)
                throw new FormatException("Invalid format for tile");

            int x = reader.ReadAsInt32() ?? 0;
            int y = reader.ReadAsInt32() ?? 0;
            double z = reader.ReadAsDouble() ?? 0.0;

            if (!reader.Read() || reader.TokenType != JsonToken.EndArray)
                throw new FormatException("Invalid format for tile");

            return new Tile(x, y, z);
        }

        public override void WriteJson(JsonWriter writer, Tile value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(value.X);
            writer.WriteValue(value.Y);
            if (Math.Abs(value.Z) >= 0.001)
                writer.WriteValue(value.Z);
            writer.WriteEndArray();
        }
    }
}
