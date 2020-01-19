using System;

using Newtonsoft.Json;

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

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
