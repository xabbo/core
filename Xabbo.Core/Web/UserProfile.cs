using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    /* Can return errors:
        {"errors":[{"param":"id","msg":"user.invalid_id","value":"..."}]}
        {"error":"not-found"}
    */

    public class UserProfile
    {
        public string UniqueId => UserInfo.UniqueId;
        public string Name => UserInfo.Name;
        public string FigureString => UserInfo.FigureString;
        public string Motto => UserInfo.Motto;
        public DateTime Created => UserInfo.Created;

        [JsonPropertyName("user")]
        public UserInfo UserInfo { get; set; } = new();

        [JsonPropertyName("groups")]
        public List<GroupInfo> Groups { get; set; } = new();

        [JsonPropertyName("badges")]
        public List<BadgeInfo> Badges { get; set; } = new();

        [JsonPropertyName("friends")]
        public List<BasicUserInfo> Friends { get; set; } = new();

        [JsonPropertyName("rooms")]
        public List<RoomInfo> Rooms { get; set; } = new();
    }
}
