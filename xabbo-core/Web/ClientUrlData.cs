using System;

using Newtonsoft.Json;

namespace Xabbo.Core.Web
{
    public class ClientUrlData
    {
        [JsonProperty("clientUrl")]
        public string ClientUrl { get; set; }
    }
}
