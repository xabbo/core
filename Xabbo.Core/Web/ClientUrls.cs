using System;
using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class ClientUrls
    {
        [JsonPropertyName("windows-version")]
        public int WindowsVersion { get; set; }

        [JsonPropertyName("osx-version")]
        public int OsxVersion { get; set; }

        [JsonPropertyName("flash-windows-version")]
        public int FlashWindowsVersion { get; set; }

        [JsonPropertyName("flash-windows")]
        public string FlashWindowsUrl { get; set; }

        [JsonPropertyName("flash-osx-version")]
        public int FlashOsxVersion { get; set; }

        [JsonPropertyName("flash-osx")]
        public string FlashOsxUrl { get; set; }

        [JsonPropertyName("unity-windows-version")]
        public int UnityWindowsVersion { get; set; }

        [JsonPropertyName("unity-windows")]
        public string UnityWindowsUrl { get; set; }

        [JsonPropertyName("unity-osx-version")]
        public int UnityOsxVersion { get; set; }

        [JsonPropertyName("unity-osx")]
        public string UnityOsxUrl { get; set; }

        public ClientUrls()
        {
            FlashWindowsUrl =
            FlashOsxUrl =
            UnityWindowsUrl =
            UnityOsxUrl = string.Empty;
        }
    }
}
