using System;

namespace Xabbo.Core;

/// <summary>
/// Defines a room moderation permission.
/// </summary>
[Flags]
public enum ModerationPermissions
{
    OwnerOnly = 0,
    RightsHolders = 1,
    AllUsers = 2,
    GroupAdmins = 4,
    GroupAdminsAndRightsHolders = RightsHolders | GroupAdmins
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
            ModerationPermissions.GroupAdmins => "Group admins",
            (ModerationPermissions.RightsHolders | ModerationPermissions.GroupAdmins)
                => "Rights holders and group admins",
            _ => "Unknown",
        };
    }
}
