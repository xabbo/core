using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Xabbo.Core.Web;

public class GameDataHashesContainer
{
    [JsonPropertyName("hashes")]
    public List<GameDataHash> Hashes { get; set; } = new();
}
