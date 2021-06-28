using System;
using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    /*
    {
        "badgeIndex":5,
        "code":"ACH_AllTimeHotelPresence7",
        "name":"Online time VII-  Cyclone",
        "description":"For spending total of 1440 min. in hotel."
    }
    */
    public class BadgeInfo
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
}
