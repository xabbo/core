using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class SessionData
    {
        [JsonPropertyName("uniqueId")]
        public string UniqueId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("figureString")]
        public string FigureString { get; set; } = string.Empty;

        [JsonPropertyName("motto")]
        public string Motto { get; set; } = string.Empty;

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
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("identityId")]
        public int IdentityId { get; set; }

        [JsonPropertyName("emailVerified")]
        public bool EmailVerified { get; set; }

        [JsonPropertyName("identityVerified")]
        public bool IdentityVerified { get; set; }

        [JsonPropertyName("identityType")]
        public string IdentityType { get; set; } = string.Empty;

        [JsonPropertyName("trusted")]
        public bool Trusted { get; set; }

        [JsonPropertyName("force")]
        public List<string> Force { get; set; } = new();

        [JsonPropertyName("accountId")]
        public int AccountId { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("traits")]
        public List<string> Traits { get; set; } = new();

        [JsonPropertyName("partner")]
        public string Partner { get; set; } = string.Empty;

        [JsonPropertyName("banned")]
        public bool Banned { get; set; }
    }
}