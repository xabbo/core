using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Xabbo.Core.Web
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LoginRequestData
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("captchaToken")]
        public string CaptchaToken { get; set; }
    }
}
