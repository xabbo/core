using System;

using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class GroupInfo
    {
        [JsonPropertyName("id")]
        public string UniqueId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("roomId")]
        public string RoomUniqueId { get; set; } = string.Empty;

        [JsonPropertyName("badgeCode")]
        public string BadgeCode { get; set; } = string.Empty;

        [JsonPropertyName("primaryColour")]
        public string PrimaryColor { get; set; } = string.Empty;

        [JsonPropertyName("secondaryColour")]
        public string SecondaryColor { get; set; } = string.Empty;
    }
}
