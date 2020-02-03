using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xabbo.Core.Metadata
{
    public sealed class ProductData : IReadOnlyDictionary<string, ProductInfo>
    {
        public static ProductData Load(string path)
        {
            using (var stream = File.OpenRead(path))
                return Load(stream);
        }

        public static ProductData Load(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var map = new Dictionary<string, ProductInfo>();

                string json;
                while ((json = reader.ReadLine()) != null)
                {
                    var products = JArray.Parse(json);
                    foreach (var product in products)
                    {
                        string[] values = product.ToObject<string[]>();
                        var info = new ProductInfo(values[0], values[1], values[2]);
                        map.Add(info.Code, info);
                    }
                }

                return new ProductData(map);
            }
        }

        private readonly Dictionary<string, ProductInfo>
            map = new Dictionary<string, ProductInfo>();

        public ProductInfo this[string key] => map[key];
        public IEnumerable<string> Keys => map.Keys;
        public IEnumerable<ProductInfo> Values => map.Values;
        public int Count => map.Count;

        private ProductData(Dictionary<string, ProductInfo> map)
        {
            this.map = map;
        }

        public bool ContainsKey(string key) => map.ContainsKey(key);
        public IEnumerator<KeyValuePair<string, ProductInfo>> GetEnumerator() => map.GetEnumerator();
        public bool TryGetValue(string key, out ProductInfo value) => map.TryGetValue(key, out value);
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
    }
}
