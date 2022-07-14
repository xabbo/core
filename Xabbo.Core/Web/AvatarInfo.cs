using System;

using System.Text.Json.Serialization;

namespace Xabbo.Core.Web;

public class AvatarInfo
{
    [JsonPropertyName("uniqueId")]
    public string UniqueId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("figureString")]
    public string FigureString { get; set; } = string.Empty;

    [JsonPropertyName("motto")]
    public string Motto { get; set; } = string.Empty;

    [JsonPropertyName("buildersClubMember")]
    public bool BuildersClubMember { get; set; }

    [JsonPropertyName("habboClubMember")]
    public bool HabboClubMember { get; set; }

    [JsonPropertyName("lastWebAccess")]
    public DateTime? LastAccess { get; set; }

    [JsonPropertyName("creationTime")]
    public DateTime Created { get; set; }

    [JsonPropertyName("banned")]
    public bool Banned { get; set; }

    [JsonPropertyName("errorMsg")]
    public string Error { get; set; } = string.Empty;
}