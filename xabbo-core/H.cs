using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Xabbo.Core.Web;

using static Xabbo.Core.XabboConst;

namespace Xabbo.Core
{
    public static class H
    {
        private static readonly Regex regexAvatarValidator
               = new Regex(@"^[a-zA-Z0-9_\-=?!@:.,]{3,15}$", RegexOptions.Compiled);

        public static bool IsValidAvatarName(string avatarName) => regexAvatarValidator.IsMatch(avatarName);

        public static Gender ToGender(int gender)
        {
            switch (gender)
            {
                // TODO Check this (from friend info)
                case 1: return Gender.Male;
                case 2: return Gender.Female;
                default: return Gender.Unisex;
            }
        }

        public static Gender ToGender(string gender)
        {
            switch (gender.ToLower())
            {
                case "m":
                case "male":
                    return Gender.Male;
                case "f":
                case "female":
                    return Gender.Female;
                case "u":
                case "unisex":
                    return Gender.Unisex;
                default: throw new FormatException($"Unknown gender: {gender}");
            }
        }

        public static FurniType ToFurniType(string s)
        {
            switch (s.ToLower())
            {
                case "s":
                case "floor":
                    return FurniType.Floor;
                case "i":
                case "wall":
                    return FurniType.Wall;
                default: throw new FormatException($"Unknown furni type: {s}");
            }
        }

        #region - Figure -
        public static bool TryGetFigurePartType(string partTypeString, out FigurePartType partType)
        {
            switch (partTypeString.ToLower())
            {
                case "hr": partType = FigurePartType.Hair; break;
                case "hd": partType = FigurePartType.Head; break;
                case "ch": partType = FigurePartType.Chest; break;
                case "lg": partType = FigurePartType.Legs; break;
                case "sh": partType = FigurePartType.Shoes; break;
                case "ha": partType = FigurePartType.Hat; break;
                case "he": partType = FigurePartType.HeadAccessory; break;
                case "ea": partType = FigurePartType.EyeAccessory; break;
                case "fa": partType = FigurePartType.FaceAccessory; break;
                case "ca": partType = FigurePartType.ChestAccessory; break;
                case "wa": partType = FigurePartType.WaistAccessory; break;
                case "cc": partType = FigurePartType.Coat; break;
                case "cp": partType = FigurePartType.ChestPrint; break;
                default: partType = default; return false;
            }
            return true;
        }

        public static FigurePartType GetFigurePartType(string partTypeString)
        {
            if (!TryGetFigurePartType(partTypeString, out FigurePartType partType))
                throw new Exception($"Unknown figure part type '{partTypeString}'");
            return partType;
        }
        #endregion

        #region - Movement -
        private static readonly int[][] directionVectors =
        {
            new[] { -1000, -10000 },
            new[] { 1000, -10000 },
            new[] { 10000, -1000 },
            new[] { 10000, 1000 },
            new[] { 1000, 10000 },
            new[] { -1000, 10000 },
            new[] { -10000, 1000 },
            new[] { -10000, -1000 }
        };

        public static (int x, int y) GetDirectionVector(int direction)
        {
            var vector = directionVectors[direction % 8];
            return (vector[0], vector[1]);
        }
        #endregion

        #region - Room -
        public static int GetHeightFromCharacter(char c)
        {
            if ('0' <= c && c <= '9')
                return c - '0';
            else if ('a' <= c && c != 'x' && c <= 'z')
                return 10 + (c - 'a');
            else
                return -1;
        }

        public static char GetCharFromHeight(int height)
        {
            if (0 <= height && height < 10)
                return (char)('0' + height);
            else if (10 <= height && height < 36)
                return (char)('a' + (height - 10));
            else
                return 'x';
        }
        #endregion

        #region - Web API -
        public static string USER_AGENT = DEFAULT_USER_AGENT;

        private static async Task<string> DownloadStringAsync(string url, string referer = "https://www.habbo.com/")
        {
            var req = WebRequest.CreateHttp(url);
            req.UserAgent = USER_AGENT;
            req.Host = "www.habbo.com";
            req.Referer = referer;

            using (var res = await req.GetResponseAsync())
            using (var reader = new StreamReader(res.GetResponseStream()))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private static async Task<byte[]> DownloadDataAsync(string url, string referer = "https://www.habbo.com/")
        {
            var req = WebRequest.CreateHttp(url);
            req.UserAgent = USER_AGENT;

            if (referer != null) req.Referer = referer;

            using (var res = await req.GetResponseAsync())
            {
                int len = (int)res.ContentLength;
                byte[] data = new byte[len];
                using (var ins = res.GetResponseStream())
                {
                    int totalRead = 0;
                    while (totalRead < len)
                    {
                        int bytesRead = await ins.ReadAsync(data, totalRead, len - totalRead);
                        if (bytesRead <= 0) throw new EndOfStreamException();
                        totalRead += bytesRead;
                    }
                }
                return data;
            }
        }

        public static Task<string> LoadExternalVariablesAsync() => DownloadStringAsync(URL_EXTERNAL_VARIABLES);

        public static async Task<UserInfo> FindUserAsync(string name)
        {
            string json = await DownloadStringAsync(API_USER_LOOKUP.Replace("$name", WebUtility.UrlEncode(name)));

            var userInfo = JsonConvert.DeserializeObject<UserInfo>(json);
            if (userInfo.UniqueId == null) return null;

            return userInfo;
        }

        public static async Task<Web.UserProfile> GetProfileAsync(string uniqueId)
        {
            string json = await DownloadStringAsync(API_USER_PROFILE.Replace("$uniqueId", uniqueId));

            var userProfile = JsonConvert.DeserializeObject<Web.UserProfile>(json);
            if (userProfile.UniqueId == null) return null;

            return userProfile;
        }

        public static Task<byte[]> DownloadFigureImageAsync(string figureString, string size = "m", int direction = 4, int headDirection = 4)
        {
            return DownloadDataAsync(
                API_FIGURE_IMAGE
                .Replace("$size", size)
                .Replace("$figure", figureString)
                .Replace("$dir", direction.ToString())
                .Replace("$headdir", headDirection.ToString())
            );
        }

        // https://habbo-stories-content.s3.amazonaws.com/servercamera/purchased/hhus/106070019-58678495-1562585848630_small.png
        public static async Task<PhotoData> GetPhotoDataAsync(string id)
        {
            string json = await DownloadStringAsync(API_FURNI_EXTRADATA.Replace("$id", id));
            return JsonConvert.DeserializeObject<PhotoData>(json);
        }

        public static async Task<byte[]> DownloadPhotoAsync(string id)
        {
            var photoData = await GetPhotoDataAsync(id);
            return await DownloadPhotoAsync(photoData);
        }

        public static Task<byte[]> DownloadPhotoAsync(PhotoData photoData)
        {
            return DownloadDataAsync(photoData.Url, null);
        }
        #endregion
    }
}
