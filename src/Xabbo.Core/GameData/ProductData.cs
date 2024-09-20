using System;
using System.IO;
using System.Text.Json;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using Xabbo.Core.Serialization;

namespace Xabbo.Core.GameData;

public sealed class ProductData : IReadOnlyDictionary<string, ProductInfo>
{
    public static ProductData LoadJsonFile(string path) => LoadJson(File.ReadAllText(path));
    public static ProductData LoadJson(string json)
    {
        return new ProductData(
            JsonSerializer.Deserialize(json, JsonProductContext.Default.ProductData)
            ?? throw new Exception("Failed to deserialize product data.")
        );
    }

    private readonly ImmutableDictionary<string, ProductInfo> _map;

    public ProductInfo this[string key] => _map[key];
    public IEnumerable<string> Keys => _map.Keys;
    public IEnumerable<ProductInfo> Values => _map.Values;
    public int Count => _map.Count;

    internal ProductData(Json.ProductData productDataProxy)
    {
        Dictionary<string, ProductInfo> map = [];

        foreach (var productInfoProxy in productDataProxy.Container.Product)
        {
            ProductInfo productInfo = new(productInfoProxy);

            if (!map.TryAdd(productInfo.Code, productInfo))
            {
                System.Diagnostics.Debug.WriteLine($"Duplicate product code '{productInfo.Code}' in product data.");
            }
        }

        _map = map.ToImmutableDictionary();
    }

    public bool ContainsKey(string key) => _map.ContainsKey(key);
    public IEnumerator<KeyValuePair<string, ProductInfo>> GetEnumerator() => _map.GetEnumerator();
    public bool TryGetValue(string key, [NotNullWhen(true)] out ProductInfo? value) => _map.TryGetValue(key, out value);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public sealed record ProductInfo(string Code, string Name, string Description)
{
    internal ProductInfo(Json.ProductInfo proxy)
        : this(proxy.Code, proxy.Name, proxy.Description)
    { }
}
