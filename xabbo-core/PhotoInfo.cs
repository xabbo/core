using System;
using Newtonsoft.Json;

namespace Xabbo.Core
{
    public class PhotoInfo
    {
        [JsonProperty("t")]
        public ulong Time { get; set; }
        
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
