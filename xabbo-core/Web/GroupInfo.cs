using System;

using Newtonsoft.Json;

namespace Xabbo.Core.Web
{
    public class GroupInfo
    {
        [JsonProperty("id")]
        public string UniqueId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("roomId")]
        public string RoomUniqueId { get; set; }

        [JsonProperty("badgeCode")]
        public string BadgeCode { get; set; }

        [JsonProperty("primaryColour")]
        public string PrimaryColor { get; set; }

        [JsonProperty("secondaryColour")]
        public string SecondaryColor { get; set; }
    }
}
