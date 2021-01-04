using System;

using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class GroupInfo
    {
        [JsonPropertyName("id")]
        public string UniqueId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("roomId")]
        public string RoomUniqueId { get; set; }

        [JsonPropertyName("badgeCode")]
        public string BadgeCode { get; set; }

        [JsonPropertyName("primaryColour")]
        public string PrimaryColor { get; set; }

        [JsonPropertyName("secondaryColour")]
        public string SecondaryColor { get; set; }
    }
}
