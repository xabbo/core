using System;
using System.Collections.Generic;

using System.Text.Json.Serialization;

namespace Xabbo.Core.Web;

public class RoomInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("creationTime")]
    public DateTime Created { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    [JsonPropertyName("maximumVisitors")]
    public int MaximumVisitors { get; set; }

    [JsonPropertyName("showOwnerName")]
    public bool ShowOwnerName { get; set; }

    [JsonPropertyName("ownerName")]
    public string OwnerName { get; set; } = string.Empty;

    [JsonPropertyName("ownerUniqueId")]
    public string OwnerUniqueId { get; set; } = string.Empty;

    [JsonPropertyName("categories")]
    public List<string> Categories { get; set; } = new();

    [JsonPropertyName("thumbnailUrl")]
    public string ThumbnailUrl { get; set; } = string.Empty;

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("rating")]
    public int Rating { get; set; }

    [JsonPropertyName("uniqueId")]
    public string UniqueId { get; set; } = string.Empty;

    // The following are not included in user profile,
    // only returned by the room info api

    [JsonPropertyName("publicRoom")]
    public bool IsPublicRoom { get; set; }

    [JsonPropertyName("doorMode")]
    public string DoorMode { get; set; } = string.Empty;
}
