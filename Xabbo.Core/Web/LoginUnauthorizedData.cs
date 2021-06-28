using System;
using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class LoginUnauthorizedData
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("captcha")]
        public bool? Captcha { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; } = string.Empty;

        [JsonPropertyName("expiryTime")]
        public DateTime? ExpiryTime { get; set; }

        [JsonPropertyName("permanentBan")]
        public bool? PermanentBan { get; set; }

        [JsonPropertyName("avatarName")]
        public string AvatarName { get; set; } = string.Empty;

        [JsonPropertyName("sanctionReasonId")]
        public string SanctionReasonId { get; set; } = string.Empty;

        [JsonPropertyName("provideExtraSupport")]
        public bool? ProvideExtraSupport { get; set; }
    }
}
