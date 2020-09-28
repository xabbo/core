using System;

using Newtonsoft.Json;

namespace Xabbo.Core.Web
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LoginUnauthorizedData
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("captcha")]
        public bool? Captcha { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("expiryTime")]
        public DateTime? ExpiryTime { get; set; }

        [JsonProperty("permanentBan")]
        public bool? PermanentBan { get; set; }

        [JsonProperty("avatarName")]
        public string AvatarName { get; set; }

        [JsonProperty("sanctionReasonId")]
        public string SanctionReasonId { get; set; }

        [JsonProperty("provideExtraSupport")]
        public bool? ProvideExtraSupport { get; set; }
    }
}
