using System;

using System.Text.Json.Serialization;

namespace Xabbo.Core.Web;

public class SelectedBadgeInfo : BadgeInfo
{
    [JsonPropertyName("badgeIndex")]
    public int Index { get; set; }
}
