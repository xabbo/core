using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xabbo.Core.Web
{
    /* Can return errors:
        {"errors":[{"param":"id","msg":"user.invalid_id","value":"..."}]}
        {"error":"not-found"}
    */

    [JsonObject(
        MemberSerialization = MemberSerialization.OptIn,
        ItemNullValueHandling = NullValueHandling.Ignore
    )]
    public class UserProfile
    {
        public string UniqueId => UserInfo.UniqueId;
        public string Name => UserInfo.Name;
        public string FigureString => UserInfo.FigureString;
        public string Motto => UserInfo.Motto;
        public DateTime Created => UserInfo.Created;

        [JsonProperty("user")]
        public UserInfo UserInfo { get; set; }

        [JsonProperty("groups")]
        public List<GroupInfo> Groups { get; set; }

        [JsonProperty("badges")]
        public List<BadgeInfo> Badges { get; set; }

        [JsonProperty("friends")]
        public List<BasicUserInfo> Friends { get; set; }

        [JsonProperty("rooms")]
        public List<RoomInfo> Rooms { get; set; }
    }
}
