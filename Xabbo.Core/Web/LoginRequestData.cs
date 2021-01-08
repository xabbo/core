using System;

using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class LoginRequestData
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("captchaToken")]
        public string CaptchaToken { get; set; }
    }
}
