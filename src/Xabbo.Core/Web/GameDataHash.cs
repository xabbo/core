﻿using System.Text.Json.Serialization;

namespace Xabbo.Core.Web;

public class GameDataHash
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("url")]
    public string Url { get; set; } = "";

    [JsonPropertyName("hash")]
    public string? Hash { get; set; }
}
