using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Xabbo.Core.Web;

public class UserInfo : BasicUserInfo
{
    [JsonPropertyName("memberSince")]
    public DateTime Created { get; set; }

    [JsonPropertyName("profileVisible")]
    public bool IsProfileVisible { get; set; }

    [JsonPropertyName("selectedBadges")]
    public List<SelectedBadgeInfo> SelectedBadges { get; set; } = new();
}
