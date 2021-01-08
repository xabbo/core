using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class SessionData
    {
        [JsonPropertyName("uniqueId")]
        public string UniqueId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("figureString")]
        public string FigureString { get; set; }

        [JsonPropertyName("motto")]
        public string Motto { get; set; }

        [JsonPropertyName("buildersClubMember")]
        public bool BuildersClubMember { get; set; }

        [JsonPropertyName("habboClubMember")]
        public bool HabboClubMember { get; set; }

        [JsonPropertyName("lastWebAccess")]
        public DateTime? LastWebAccess { get; set; }

        [JsonPropertyName("creationTime")]
        public DateTime CreationTime { get; set; }

        [JsonPropertyName("sessionLogId")]
        public long SessionLogId { get; set; }

        [JsonPropertyName("loginLogId")]
        public long LoginLogId { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("identityId")]
        public int IdentityId { get; set; }

        [JsonPropertyName("emailVerified")]
        public bool EmailVerified { get; set; }

        [JsonPropertyName("identityVerified")]
        public bool IdentityVerified { get; set; }

        [JsonPropertyName("identityType")]
        public string IdentityType { get; set; }

        [JsonPropertyName("trusted")]
        public bool Trusted { get; set; }

        [JsonPropertyName("force")]
        public List<string> Force { get; set; }

        [JsonPropertyName("accountId")]
        public int AccountId { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("traits")]
        public List<string> Traits { get; set; }

        [JsonPropertyName("partner")]
        public string Partner { get; set; }

        [JsonPropertyName("banned")]
        public bool Banned { get; set; }
    }
}