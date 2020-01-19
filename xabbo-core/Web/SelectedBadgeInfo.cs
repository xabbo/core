using System;

using Newtonsoft.Json;

namespace Xabbo.Core.Web
{
    public class SelectedBadgeInfo : BadgeInfo
    {
        [JsonProperty("badgeIndex")]
        public int Index { get; set; }
    }
}
