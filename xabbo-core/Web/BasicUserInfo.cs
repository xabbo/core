using System;
using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class BasicUserInfo
    {
        [JsonPropertyName("uniqueId")]
        public string UniqueId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("figureString")]
        public string FigureString { get; set; }

        [JsonPropertyName("motto")]
        public string Motto { get; set; }
    }
}
