using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xabbo.Core.GameData.Json;

public class ProductData
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public static ProductData? Load(string json) => JsonSerializer.Deserialize<ProductData>(json, _serializerOptions);

    [JsonPropertyName("productdata")]
    public ProductInfoContainer Container { get; set; } = new();

    public class ProductInfoContainer
    {
        public List<ProductInfo> Product { get; set; } = new();
    }
}

public class ProductInfo
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
