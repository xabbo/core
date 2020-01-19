using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Xabbo.Core.Web
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ErrorInfos
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("errors")]
        public List<ErrorInfo> Errors { get; set; }
    }
}
