using System;
using System.Collections.Generic;

using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class RoomInfo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("creationTime")]
        public DateTime Created { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }

        [JsonPropertyName("maximumVisitors")]
        public int MaximumVisitors { get; set; }

        [JsonPropertyName("showOwnerName")]
        public bool ShowOwnerName { get; set; }

        [JsonPropertyName("ownerName")]
        public string OwnerName { get; set; }

        [JsonPropertyName("ownerUniqueId")]
        public string OwnerUniqueId { get; set; }

        [JsonPropertyName("categories")]
        public List<string> Categories { get; set; }

        [JsonPropertyName("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("rating")]
        public int Rating { get; set; }

        [JsonPropertyName("uniqueId")]
        public string UniqueId { get; set; }

        // The following are not included in user profile,
        // only returned by the room info api

        [JsonPropertyName("publicRoom")]
        public bool IsPublicRoom { get; set; }

        [JsonPropertyName("doorMode")]
        public string DoorMode { get; set; }
    }
}
