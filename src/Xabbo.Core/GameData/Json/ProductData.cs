using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

using Xabbo.Core.Serialization;

namespace Xabbo.Core.GameData.Json;

public class ProductData
{
    public static ProductData? Load(string json) => JsonSerializer.Deserialize(json, JsonProductContext.Default.ProductData);

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
