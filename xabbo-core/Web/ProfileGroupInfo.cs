using System;

using Newtonsoft.Json;

namespace Xabbo.Core.Web
{
    public class ProfileGroupInfo : GroupInfo
    {
        [JsonProperty("isAdmin")]
        public bool IsAdmin { get; set; }
    }
}
