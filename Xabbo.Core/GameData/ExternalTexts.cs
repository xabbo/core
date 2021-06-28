using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Xabbo.Core.GameData
{
    public class ExternalTexts : IReadOnlyDictionary<string, string>
    {
        public static ExternalTexts Load(string path) => new(path);

        private readonly Dictionary<string, string> dict = new();

        public IEnumerable<string> Keys => dict.Keys;
        public IEnumerable<string> Values => dict.Values;
        public int Count => dict.Count;
        public string this[string key] => dict[key];

        internal ExternalTexts(string path)
        {
            foreach (string line in File.ReadLines(path))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                int index = line.IndexOf('=');
                if (index < 0)
                {
                    DebugUtil.Log($"invalid line {line}");
                    continue;
                }

                string key = line.Substring(0, index);
                string value = line[(index + 1)..];
                if (!dict.ContainsKey(key))
                    dict.Add(key, value);
                else
                    DebugUtil.Log($"duplicate key '{key}'");
            }
        }

        public bool ContainsKey(string key) => dict.ContainsKey(key);
        public bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => dict.TryGetValue(key, out value);
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => dict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
