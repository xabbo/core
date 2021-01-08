using System;

using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class AvatarInfo
    {
        [JsonPropertyName("uniqueId")]
        public string UniqueId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("figureString")]
        public string FigureString { get; set; }

        [JsonPropertyName("motto")]
        public string Motto { get; set; }

        [JsonPropertyName("buildersClubMember")]
        public bool BuildersClubMember { get; set; }

        [JsonPropertyName("habboClubMember")]
        public bool HabboClubMember { get; set; }

        [JsonPropertyName("lastWebAccess")]
        public DateTime? LastAccess { get; set; }

        [JsonPropertyName("creationTime")]
        public DateTime Created { get; set; }

        [JsonPropertyName("banned")]
        public bool Banned { get; set; }

        [JsonPropertyName("errorMsg")]
        public string Error { get; set; }
    }
}