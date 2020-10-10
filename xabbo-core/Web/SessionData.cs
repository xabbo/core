using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Xabbo.Core.Web
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class SessionData
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
        public DateTime? LastWebAccess { get; set; }

        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }

        [JsonProperty("sessionLogId")]
        public long SessionLogId { get; set; }

        [JsonProperty("loginLogId")]
        public long LoginLogId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("identityId")]
        public int IdentityId { get; set; }

        [JsonProperty("emailVerified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("identityVerified")]
        public bool IdentityVerified { get; set; }

        [JsonProperty("identityType")]
        public string IdentityType { get; set; }

        [JsonProperty("trusted")]
        public bool Trusted { get; set; }

        [JsonProperty("force")]
        public List<string> Force { get; set; }

        [JsonProperty("accountId")]
        public int AccountId { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("traits")]
        public List<string> Traits { get; set; }

        [JsonProperty("partner")]
        public string Partner { get; set; }

        [JsonProperty("banned")]
        public bool Banned { get; set; }
    }
}