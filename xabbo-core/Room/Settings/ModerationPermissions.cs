using System;
using System.ComponentModel;
using System.Reflection;

namespace Xabbo.Core
{
    public enum ModerationPermissions
    {
        [Description("Owner only")]
        OwnerOnly = 0,
        [Description("Rights holders")]
        RightsHolders = 1,
        [Description("All users")]
        AllUsers = 2
    }

    public static partial class XabboEnumExtensions
    {
        public static string ToFriendlyName(this ModerationPermissions permissions)
        {
            switch (permissions)
            {
                case ModerationPermissions.OwnerOnly: return "Owner only";
                case ModerationPermissions.RightsHolders: return "Rights holders";
                case ModerationPermissions.AllUsers: return "All users";
                default: return "Unknown";
            }
        }
    }
}
