using System;
using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class LoginUnauthorizedData
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("captcha")]
        public bool? Captcha { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("expiryTime")]
        public DateTime? ExpiryTime { get; set; }

        [JsonPropertyName("permanentBan")]
        public bool? PermanentBan { get; set; }

        [JsonPropertyName("avatarName")]
        public string AvatarName { get; set; }

        [JsonPropertyName("sanctionReasonId")]
        public string SanctionReasonId { get; set; }

        [JsonPropertyName("provideExtraSupport")]
        public bool? ProvideExtraSupport { get; set; }
    }
}
