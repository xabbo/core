using System;

namespace Xabbo.Core
{
    internal class XabboConst
    {
        public const double DEFAULT_EPSILON = 0.0001;

        public const string DEFAULT_USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.97 Safari/537.36";
        public const string
            URL_EXTERNAL_VARIABLES = "https://www.habbo.com/gamedata/external_variables/1",
            API_USER_LOOKUP = "https://www.habbo.com/api/public/users?name=$name",
            API_USER_PROFILE = "https://www.habbo.com/api/public/users/$uniqueId/profile",
            API_FIGURE_IMAGE = "https://www.habbo.com/habbo-imaging/avatarimage?size=$size&figure=$figure&direction=$dir&head_direction=$headdir",
            API_FURNI_EXTRADATA = "https://extradata.habbo.com/public/furni/$id";
    }
}
