using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Xabbo.Core.Web
{
    /* Example data
    {
        "uniqueId":"hhus-855a10ddd250dd513830f467b510f3e1",
        "name":"a36ef14e",
        "figureString":"hr-892-46.hd-209-8.ch-210-1408.lg-280-1408.sh-906-68.ha-1003-80.cc-260-1408",
        "motto":"",
        "buildersClubMember":false,
        "habboClubMember":false,
        "lastWebAccess":"2018-05-20T15:52:12.000+0000",
        "creationTime":"2018-05-19T14:19:36.000+0000",
        "sessionLogId":152683163141831650,
        "loginLogId":152683163141831740,
        "email":"ef1ca2c4@gmail.com",
        "identityId":26827919,
        "emailVerified":true,
        "identityVerified":true,
        "identityType":"HABBO",
        "trusted":true,
        "force":["NONE"],
        "accountId":64440815,
        "country":"nz",
        "traits":["USER"],
        "partner":"NO_PARTNER"
    }
    */

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LoginResponseData
    {
        [JsonProperty("uniqueId")]
        public string UniqueId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("figureString")]
        public string FigureString { get; set; }

        [JsonProperty("motto")]
        public string Motto { get; set; }

        [JsonProperty("buildersClubMember")]
        public bool BuildersClubMember { get; set; }

        [JsonProperty("habboClubMember")]
        public bool HabboClubMember { get; set; }

        [JsonProperty("lastWebAccess")]
        public DateTime LastWebAccess { get; set; }

        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }

        [JsonProperty("sessionLogId")]
        public long SessionLogId { get; set; }

        [JsonProperty("loginLogId")]
        public long LoginLogId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("identityId")]
        public int IdentityId { get; set; }

        [JsonProperty("emailVerified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("identityVerified")]
        public bool IdentityVerified { get; set; }

        [JsonProperty("identityType")]
        public string IdentityType { get; set; }

        [JsonProperty("trusted")]
        public bool Trusted { get; set; }

        [JsonProperty("force")]
        public List<string> Force { get; set; }

        [JsonProperty("accountId")]
        public int AccountId { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("traits")]
        public List<string> Traits { get; set; }

        [JsonProperty("partner")]
        public string Partner { get; set; }

        [JsonProperty("banned")]
        public bool Banned { get; set; }
    }
}