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
        public static string GetValue(this BanDuration banType)
        {
            switch (banType)
            {
                case BanDuration.Hour: return "RWUAM_BAN_USER_HOUR";
                case BanDuration.Day: return "RWUAM_BAN_USER_DAY";
                case BanDuration.Permanent: return "RWUAM_BAN_USER_PERM";
                default: return null;
            }
        }
    }
}
