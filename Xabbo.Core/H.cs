using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                case 0: return Gender.Female;
                case 1: return Gender.Male;
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

        public static ItemType ToItemType(string s)
        {
            if (s.Length != 1)
                throw new Exception($"Invalid item type: {s}");

            ItemType type = (ItemType)s.ToLower()[0];
            if (!Enum.IsDefined(typeof(ItemType), type))
                throw new Exception($"Unknown item type: {s}");

            return type;
        }

        public static ItemType ToItemType(int value)
        {
            return value switch
            {
                0 => ItemType.Wall,
                1 => ItemType.Floor,
                _ => throw new Exception($"Unknown item type {value}")
            };
        }

        public static ItemType ToItemType(char value)
        {
            return value switch
            {
                's' => ItemType.Floor,
                'i' => ItemType.Wall,
                'e' => ItemType.Effect,
                'b' => ItemType.Badge,
                'r' => ItemType.Bot,
                _ => throw new Exception($"Unknown item type '{value}'")
            };
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
            if (TryGetFigurePartType(partTypeString, out FigurePartType partType))
                return partType;

            throw new Exception($"Unknown figure part type '{partTypeString}'");
        }
        #endregion

        #region - Movement -
        private static readonly int[][] magicVectors =
        {
            new[] { -1000, -10000 }, // N
            new[] { 1000, -10000 },  // NE
            new[] { 10000, -1000 },  // E
            new[] { 10000, 1000 },   // SE
            new[] { 1000, 10000 },   // S
            new[] { -1000, 10000 },  // SW
            new[] { -10000, 1000 },  // W
            new[] { -10000, -1000 }  // NW
        };

        /// <summary>
        /// Gets a vector that can be used to face the specified direction
        /// regardless of where your character is in the room.
        /// </summary>
        /// <param name="direction">The direction to face.</param>
        public static (int X, int Y) GetMagicVector(int direction)
        {
            var vector = magicVectors[direction % 8];
            return (vector[0], vector[1]);
        }

        public static (int X, int Y) GetMagicVector(Directions direction) => GetMagicVector((int)direction);

        public static (int X, int Y) GetVector(Directions direction)
        {
            switch (direction)
            {
                case Directions.North: return (0, -1);
                case Directions.NorthEast: return (1, -1);
                case Directions.East: return (1, 0);
                case Directions.SouthEast: return (1, 1);
                case Directions.South: return (0, 1);
                case Directions.SouthWest: return (-1, 1);
                case Directions.West: return (-1, 0);
                case Directions.NorthWest: return (-1, -1);
                default: return (0, 0);
            }
        }

        public static (int X, int Y) GetVector(int direction) => GetVector((Directions)direction);
        #endregion

        #region - Room -
        public static int GetHeightFromCharacter(char c)
        {
            if ('0' <= c && c <= '9')
                return c - '0';
            else if ('a' <= c && c != 'x' && c <= 'z')
                return 10 + (c - 'a');
            else if ('A' <= c && c != 'X' && c <= 'Z')
                return 10 + (c - 'A');
            else
                return -110;
        }

        public static char GetCharacterFromHeight(int height)
        {
            if (0 <= height && height < 10)
                return (char)('0' + height);
            else if (10 <= height && height < 36)
                return (char)('a' + (height - 10));
            else
                return 'x';
        }
        #endregion

        #region - Text -
        private static readonly Dictionary<char, string> altCharacterMap = new Dictionary<char, string>()
        {
            { '‡', "🚫" },
            { '|', "❤️" },
            { '¥', "⭐" },
            { 'ƒ', "🖤" },
            { 'í', "-" },
            { '—', "🎵" },
            { 'ª', "💀" },
            { 'º', "⚡" },
            { 'µ', "☕" },
            { '±', "📱" },
            { '÷', "👎" },
            { '•', "👍" },
            { '¶', "💡" },
            { '‘', "🔒" },
            { '†', "💣" },
            { '¬', "🐟" },
            { '»', "♣️" }
        };

        public static IReadOnlyDictionary<char, string> GetAltCharacterMap() => altCharacterMap.ToDictionary(x => x.Key, x => x.Value);

        public static string ReplaceSpecialCharacters(string text)
        {
            var sb = new StringBuilder();
            foreach (char c in text)
            {
                if (altCharacterMap.ContainsKey(c))
                    sb.Append(altCharacterMap[c]);
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }
        #endregion

        #region - Web API -
        public static string USER_AGENT = DEFAULT_USER_AGENT;

        private static async Task<string> DownloadStringAsync(string url, string referer = "https://www.habbo.com/")
        {
            var req = WebRequest.CreateHttp(url);
            req.UserAgent = USER_AGENT;
            //req.Host = "www.habbo.com";
            req.Referer = referer;

            using (var res = await req.GetResponseAsync().ConfigureAwait(false))
            using (var reader = new StreamReader(res.GetResponseStream()))
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        private static async Task<byte[]> DownloadDataAsync(string url, string referer = "https://www.habbo.com/")
        {
            var req = WebRequest.CreateHttp(url);
            req.UserAgent = USER_AGENT;

            if (referer != null) req.Referer = referer;

            using (var res = await req.GetResponseAsync().ConfigureAwait(false))
            {
                int len = (int)res.ContentLength;
                byte[] data = new byte[len];
                using (var ins = res.GetResponseStream())
                {
                    int totalRead = 0;
                    while (totalRead < len)
                    {
                        int bytesRead = await ins.ReadAsync(data, totalRead, len - totalRead).ConfigureAwait(false);
                        if (bytesRead <= 0) throw new EndOfStreamException();
                        totalRead += bytesRead;
                    }
                }
                return data;
            }
        }

        public static Task<string> LoadExternalVariablesAsync() => LoadExternalVariablesAsync("com"); 

        public static Task<string> LoadExternalVariablesAsync(string domain)
            => DownloadStringAsync(URL_EXTERNAL_VARIABLES.Replace("$domain", domain));

        public static async Task<UserInfo> FindUserAsync(string name)
        {
            string json = await DownloadStringAsync(API_USER_LOOKUP.Replace("$name", WebUtility.UrlEncode(name))).ConfigureAwait(false);

            var userInfo = JsonSerializer.Deserialize<UserInfo>(json);
            if (userInfo.UniqueId == null) return null;

            return userInfo;
        }

        public static async Task<Web.UserProfile> GetProfileAsync(string uniqueId)
        {
            string json = await DownloadStringAsync(API_USER_PROFILE.Replace("$uniqueId", uniqueId)).ConfigureAwait(false);

            var userProfile = JsonSerializer.Deserialize<Web.UserProfile>(json);
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
            string json = await DownloadStringAsync(API_FURNI_EXTRADATA.Replace("$id", id)).ConfigureAwait(false);
            return JsonSerializer.Deserialize<PhotoData>(json);
        }

        public static async Task<byte[]> DownloadPhotoAsync(string id)
        {
            var photoData = await GetPhotoDataAsync(id).ConfigureAwait(false);
            return await DownloadPhotoAsync(photoData).ConfigureAwait(false);
        }

        public static Task<byte[]> DownloadPhotoAsync(PhotoData photoData)
        {
            return DownloadDataAsync(photoData.Url, null);
        }
        #endregion
    }
}
