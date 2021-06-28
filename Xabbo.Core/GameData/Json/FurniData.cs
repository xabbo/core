using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Xabbo.Core.GameData.Json
{
    public class FurniData
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public static FurniData Load(string json) => JsonSerializer.Deserialize<FurniData>(json, _options)
            ?? throw new Exception("Failed to deserialize furni data.");

        public FurniInfoContainer RoomItemTypes { get; set; } = new();
        public FurniInfoContainer WallItemTypes { get; set; } = new();

        public class FurniInfoContainer
        {
            public List<FurniInfo> FurniType { get; set; }

            public FurniInfoContainer()
            {
                FurniType = new List<FurniInfo>();
            }
        }
    }

    public class FurniInfo
    {
        public int Id { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int Revision { get; set; }
        public string Category { get; set; } = string.Empty;
        public int DefaultDir { get; set; }
        public int XDim { get; set; }
        public int YDim { get; set; }
        public PartColorContainer PartColors { get; set; } = new();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AdUrl { get; set; } = string.Empty;
        public int OfferId { get; set; }
        public bool Buyout { get; set; }
        public int RentOfferId { get; set; }
        public bool RentBuyout { get; set; }
        public bool BC { get; set; }
        public bool ExcludedDynamic { get; set; }
        public string CustomParams { get; set; } = string.Empty;
        public int SpecialType { get; set; }
        public bool CanStandOn { get; set; }
        public bool CanSitOn { get; set; }
        public bool CanLayOn { get; set; }
        public string FurniLine { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public bool Rare { get; set; }

        public class PartColorContainer
        {
            public string[] Colors { get; set; } = Array.Empty<string>();
        }
    }
}
