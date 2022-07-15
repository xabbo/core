using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Xabbo.Core.Web;

internal class GameDataHashesContainer
{
    [JsonPropertyName("hashes")]
    public List<GameDataHash> Hashes { get; set; } = new();
}
