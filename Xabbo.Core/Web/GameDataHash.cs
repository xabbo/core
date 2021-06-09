using System;
using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class GameDataHash
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("hash")]
        public string Hash { get; set; } = string.Empty;
    }
}
