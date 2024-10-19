using System.Text.Json.Serialization;

namespace Xabbo.Core;

public class PhotoInfo
{
    [JsonPropertyName("t")]
    public ulong Time { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    public PhotoInfo()
    {
        Id = "";
    }
}
