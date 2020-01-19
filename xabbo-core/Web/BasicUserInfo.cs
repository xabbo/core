using System;

using Newtonsoft.Json;

namespace Xabbo.Core.Web
{
    [JsonObject(
        MemberSerialization = MemberSerialization.OptIn,
        ItemNullValueHandling = NullValueHandling.Ignore
    )]
    public class BasicUserInfo
    {
        [JsonProperty("uniqueId")]
        public string UniqueId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("figureString")]
        public string FigureString { get; set; }

        [JsonProperty("motto")]
        public string Motto { get; set; }
    }
}
