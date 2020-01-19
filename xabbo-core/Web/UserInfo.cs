using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Xabbo.Core.Web
{
    [JsonObject(
        MemberSerialization = MemberSerialization.OptIn,
        ItemNullValueHandling = NullValueHandling.Ignore
    )]
    public class UserInfo : BasicUserInfo
    {
        [JsonProperty("memberSince")]
        public DateTime Created { get; set; }

        [JsonProperty("profileVisible")]
        public bool IsProfileVisible { get; set; }

        [JsonProperty("selectedBadges")]
        public List<SelectedBadgeInfo> SelectedBadges { get; set; }
    }
}
