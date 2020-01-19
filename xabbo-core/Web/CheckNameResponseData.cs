using System;

using Newtonsoft.Json;

namespace Xabbo.Core.Web
{
    public class CheckNameResponse
    {
        [JsonProperty("isAvailable")]
        public bool IsAvailable { get; set; }
    }
}
