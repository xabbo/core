using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Xabbo.Core.Web
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PhotoData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("room_id")]
        public int RoomId { get; set; }

        [JsonProperty("creator_uniqueId")]
        public string CreatorUniqueId { get; set; }

        [JsonProperty("creator_id")]
        public int CreatorId { get; set; }

        [JsonProperty("creator_name")]
        public string CreatorName { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("previewUrl")]
        public string PreviewUrl { get; set; }
    }
}
