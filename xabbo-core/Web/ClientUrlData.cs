using System;

using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class ClientUrlData
    {
        [JsonPropertyName("clientUrl")]
        public string ClientUrl { get; set; }
    }
}
