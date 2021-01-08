using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xabbo.Core.GameData.Json
{
    public class ProductData
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public static ProductData Load(string json) => JsonSerializer.Deserialize<ProductData>(json, _options);

        [JsonPropertyName("productdata")]
        public ProductInfoContainer Container { get; set; }

        public class ProductInfoContainer
        {
            public List<ProductInfo> Product { get; set; }

            public ProductInfoContainer()
            {
                Product = new List<ProductInfo>();
            }
        }
    }

    public class ProductInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
