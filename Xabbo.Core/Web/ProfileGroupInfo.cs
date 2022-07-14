using System;

using System.Text.Json.Serialization;

namespace Xabbo.Core.Web;

public class ProfileGroupInfo : GroupInfo
{
    [JsonPropertyName("isAdmin")]
    public bool IsAdmin { get; set; }
}
