using System;

using System.Text.Json.Serialization;

namespace Xabbo.Core.Web;

public class CheckNameResponse
{
    [JsonPropertyName("isAvailable")]
    public bool IsAvailable { get; set; }
}
