using System;

using Newtonsoft.Json;

namespace Xabbo.Core.Web
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AvatarInfo
    {
        [JsonProperty("uniqueId")]
        public string UniqueId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("figureString")]
        public string FigureString { get; set; }

        [JsonProperty("motto")]
        public string Motto { get; set; }

        [JsonProperty("buildersClubMember")]
        public bool BuildersClubMember { get; set; }

        [JsonProperty("habboClubMember")]
        public bool HabboClubMember { get; set; }

        [JsonProperty("lastWebAccess")]
        public DateTime? LastAccess { get; set; }

        [JsonProperty("creationTime")]
        public DateTime Created { get; set; }

        [JsonProperty("banned")]
        public bool Banned { get; set; }

        [JsonProperty("errorMsg")]
        public string Error { get; set; }
    }
}