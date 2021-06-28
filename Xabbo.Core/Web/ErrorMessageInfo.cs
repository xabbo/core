using System;
using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class ErrorMessageInfo
    {
        [JsonPropertyName("errorMsg")]
        public string ErrorMsg { get; set; } = string.Empty;
    }
}
