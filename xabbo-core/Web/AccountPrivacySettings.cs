using System;
using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class AccountSettings
    {
        [JsonPropertyName("profileVisible")]
        public bool IsProfileVisible { get; set; }

        [JsonPropertyName("onlineStatusVisible")]
        public bool ShowOnline { get; set; }

        [JsonPropertyName("friendCanFollow")]
        public bool AllowFollow { get; set; }

        [JsonPropertyName("friendRequestEnabled")]
        public bool AllowFriendRequests { get; set; }

        [JsonPropertyName("offlineMessagingEnabled")]
        public bool AllowOfflineMessaging { get; set; }

        [JsonPropertyName("emailNewsletterEnabled")]
        public bool ReceiveNewsletter { get; set; }

        [JsonPropertyName("emailMiniMailNotificationEnabled")]
        public bool NotifyMiniMail { get; set; }

        [JsonPropertyName("emailFriendRequestNotificationEnabled")]
        public bool NotifyFriendRequest { get; set; }

        [JsonPropertyName("emailGiftNotificationEnabled")]
        public bool NotifyGift { get; set; }

        [JsonPropertyName("emailRoomMessageNotificationEnabled")]
        public bool NotifyRoomMessage { get; set; }

        [JsonPropertyName("emailGroupNotificationEnabled")]
        public bool NotifyGroupRequest { get; set; }

        [JsonPropertyName("gdprDataReady")]
        public bool GDPRDataReady { get; set; }
    }
}
