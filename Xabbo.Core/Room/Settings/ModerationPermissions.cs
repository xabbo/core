using System;

namespace Xabbo.Core
{
    public enum ModerationPermissions
    {
        OwnerOnly = 0,
        RightsHolders = 1,
        AllUsers = 2
    }

    public static partial class XabboEnumExtensions
    {
        public static string ToFriendlyName(this ModerationPermissions permissions)
        {
            return permissions switch
            {
                ModerationPermissions.OwnerOnly => "Owner only",
                ModerationPermissions.RightsHolders => "Rights holders",
                ModerationPermissions.AllUsers => "All users",
                _ => "Unknown",
            };
        }
    }
}
