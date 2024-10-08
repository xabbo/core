﻿using System;
using System.IO;
using System.Text.Json;
using System.Collections;
using System.Collections.Generic;
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

    private readonly Dictionary<string, ProductInfo> _map = new();

    public ProductInfo this[string key] => _map[key];
    public IEnumerable<string> Keys => _map.Keys;
    public IEnumerable<ProductInfo> Values => _map.Values;
    public int Count => _map.Count;

    internal ProductData(Json.ProductData productDataProxy)
    {
        foreach (var productInfoProxy in productDataProxy.Container.Product)
        {
            ProductInfo productInfo = new(productInfoProxy);

            if (_map.ContainsKey(productInfo.Code))
            {
                System.Diagnostics.Debug.WriteLine($"[!] Duplicate product code '{productInfo.Code}' in JSON product data.");
            }
            else
            {
                _map.Add(productInfo.Code, productInfo);
            }
        }
    }

    public bool ContainsKey(string key) => _map.ContainsKey(key);
    public IEnumerator<KeyValuePair<string, ProductInfo>> GetEnumerator() => _map.GetEnumerator();
    public bool TryGetValue(string key, [NotNullWhen(true)] out ProductInfo? value) => _map.TryGetValue(key, out value);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class ProductInfo
{
    public string Code { get; }
    public string Name { get; }
    public string Description { get; }

    public ProductInfo(string code, string name, string description)
    {
        Code = code;
        Name = name;
        Description = description;
    }

    internal ProductInfo(Json.ProductInfo proxy)
    {
        Code = proxy.Code;
        Name = proxy.Name;
        Description = proxy.Description;
    }
}
