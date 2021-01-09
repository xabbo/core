﻿using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Xabbo.Core.GameData.Json
{
    public class FurniData
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public static FurniData Load(string json) => JsonSerializer.Deserialize<FurniData>(json, _options);

        public FurniInfoContainer RoomItemTypes { get; set; }
        public FurniInfoContainer WallItemTypes { get; set; }

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
        public string ClassName { get; set; }
        public int Revision { get; set; }
        public string Category { get; set; }
        public int DefaultDir { get; set; }
        public int XDim { get; set; }
        public int YDim { get; set; }
        public PartColorContainer PartColors { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AdUrl { get; set; }
        public int OfferId { get; set; }
        public bool Buyout { get; set; }
        public int RentOfferId { get; set; }
        public bool RentBuyout { get; set; }
        public bool BC { get; set; }
        public bool ExcludedDynamic { get; set; }
        public string CustomParams { get; set; }
        public int SpecialType { get; set; }
        public bool CanStandOn { get; set; }
        public bool CanSitOn { get; set; }
        public bool CanLayOn { get; set; }
        public string FurniLine { get; set; }
        public string Environment { get; set; }
        public bool Rare { get; set; }

        public class PartColorContainer
        {
            public string[] Colors { get; set; }

            public PartColorContainer()
            {
                Colors = new string[0];
            }
        }
    }
}