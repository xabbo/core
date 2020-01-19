using System;
using Newtonsoft.Json;

namespace Xabbo.Core.Web
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ErrorMessageInfo
    {
        [JsonProperty("errorMsg")]
        public string ErrorMsg { get; set; }
    }
}
