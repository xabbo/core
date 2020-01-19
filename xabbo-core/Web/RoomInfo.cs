using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Xabbo.Core.Web
{
    public class RoomInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("creationTime")]
        public DateTime Created { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("maximumVisitors")]
        public int MaximumVisitors { get; set; }

        [JsonProperty("showOwnerName")]
        public bool ShowOwnerName { get; set; }

        [JsonProperty("ownerName")]
        public string OwnerName { get; set; }

        [JsonProperty("ownerUniqueId")]
        public string OwnerUniqueId { get; set; }

        [JsonProperty("categories")]
        public List<string> Categories { get; set; }

        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("rating")]
        public int Rating { get; set; }

        [JsonProperty("uniqueId")]
        public string UniqueId { get; set; }

        // The following are not included in user profile,
        // only returned by the room info api

        [JsonProperty("publicRoom")]
        public bool IsPublicRoom { get; set; }

        [JsonProperty("doorMode")]
        public string DoorMode { get; set; }
    }
}
