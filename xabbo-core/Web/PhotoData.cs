using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xabbo.Core.Web
{
    /*
    {
       "previewUrl":"https://habbo-stories-content.s3.amazonaws.com/servercamera/purchased/hhus/98107322-34484510-1504179191115.png",
       "tags":[

       ],
       "creator_uniqueId":"hhus-15c85e4918344ea82b9fc1150e43cc2d",
       "type":"PHOTO",
       "url":"https://habbo-stories-content.s3.amazonaws.com/servercamera/purchased/hhus/98107322-34484510-1504179191115.png",
       "version":1,
       "time":1504179191592,
       "creator_name":"steeliestuff",
       "creator_id":34484510,
       "room_id":73506662,
       "id":"5b17cdd9-0320-4ec3-8a2f-22de6766bd40"
    }
    */
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
        public ulong Time { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

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
