using System;
using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class BasicUserInfo
    {
        [JsonPropertyName("uniqueId")]
        public string UniqueId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("figureString")]
        public string FigureString { get; set; } = string.Empty;

        [JsonPropertyName("motto")]
        public string Motto { get; set; } = string.Empty;
    }
}
