using System;

namespace Xabbo.Core;

internal static class XabboConst
{
    public const int DefaultTimeout = 10000;
    public const int DefaultOriginsInventoryScanInterval = 1000;
    public const float DefaultEpsilon = 0.0015f;

    public const string DefaultUserAgent =
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.5112.79 Safari/537.36";
    public const string
        URL_EXTERNAL_VARIABLES = "https://www.habbo.$domain/gamedata/external_variables/1",
        API_USER_LOOKUP = "https://www.habbo.com/api/public/users?name=$name",
        API_USER_PROFILE = "https://www.habbo.com/api/public/users/$uniqueId/profile",
        API_FIGURE_IMAGE = "https://www.habbo.com/habbo-imaging/avatarimage?size=$size&figure=$figure&direction=$dir&head_direction=$headdir",
        API_FURNI_EXTRADATA = "https://extradata.habbo.com/public/furni/$id";
}
