using System;

using Newtonsoft.Json;

namespace Xabbo.Core.Web
{
    /*
    {
       "profileVisible":false,
       "onlineStatusVisible":true,
       "friendCanFollow":true,
       "friendRequestEnabled":true,
       "offlineMessagingEnabled":true,
       "emailNewsletterEnabled":false,
       "emailMiniMailNotificationEnabled":true,
       "emailFriendRequestNotificationEnabled":true,
       "emailGiftNotificationEnabled":true,
       "emailRoomMessageNotificationEnabled":true,
       "emailGroupNotificationEnabled":true,
       "gdprDataReady":false
    }
    */
    public class AccountSettings
    {
        [JsonProperty("profileVisible")]
        public bool ProfileVisible { get; set; }

        [JsonProperty("onlineStatusVisible")]
        public bool ShowOnline { get; set; }

        [JsonProperty("friendCanFollow")]
        public bool AllowFollow { get; set; }

        [JsonProperty("friendRequestEnabled")]
        public bool AllowFriendRequests { get; set; }

        [JsonProperty("offlineMessagingEnabled")]
        public bool AllowOfflineMessaging { get; set; }

        [JsonProperty("emailNewsletterEnabled")]
        public bool ReceiveNewsletter { get; set; }

        [JsonProperty("emailMiniMailNotificationEnabled")]
        public bool NotifyMiniMail { get; set; }

        [JsonProperty("emailFriendRequestNotificationEnabled")]
        public bool NotifyFriendRequest { get; set; }

        [JsonProperty("emailGiftNotificationEnabled")]
        public bool NotifyGift { get; set; }

        [JsonProperty("emailRoomMessageNotificationEnabled")]
        public bool NotifyRoomMessage { get; set; }

        [JsonProperty("emailGroupNotificationEnabled")]
        public bool NotifyGroupRequest { get; set; }

        [JsonProperty("gdprDataReady")]
        public bool GDPRDataReady { get; set; }
    }
}
