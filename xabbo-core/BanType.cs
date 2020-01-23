using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xabbo.Core
{
    public enum BanType
    {
        Hour,
        Day,
        Permanent
    }

    public static partial class XabboEnumExtensions
    {
        public static string GetValue(this BanType banType)
        {
            switch (banType)
            {
                case BanType.Hour: return "RWUAM_BAN_USER_HOUR";
                case BanType.Day: return "RWUAM_BAN_USER_DAY";
                case BanType.Permanent: return "RWUAM_BAN_USER_PERM";
                default: return null;
            }
        }
    }
}
