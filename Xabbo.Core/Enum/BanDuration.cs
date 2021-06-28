using System;

namespace Xabbo.Core
{
    public enum BanDuration
    {
        Hour,
        Day,
        Permanent
    }

    public static partial class XabboEnumExtensions
    {
        public static string GetValue(this BanDuration banDuration)
        {
            return banDuration switch
            {
                BanDuration.Hour => "RWUAM_BAN_USER_HOUR",
                BanDuration.Day => "RWUAM_BAN_USER_DAY",
                BanDuration.Permanent => "RWUAM_BAN_USER_PERM",
                _ => throw new Exception($"Unknown ban duration: {banDuration}.")
            };
        }
    }
}
