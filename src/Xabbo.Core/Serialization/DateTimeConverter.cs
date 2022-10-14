using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xabbo.Core.Serialization;

internal class DateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? dateString = reader.GetString();
        return dateString is null ? null : DateTime.Parse(dateString);
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString("yyyy-MM-dd'T'HH:mm:ss.fff+0000"));
    }
}
