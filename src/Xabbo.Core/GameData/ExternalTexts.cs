using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Immutable;

namespace Xabbo.Core.GameData;

public sealed class ExternalTexts : IReadOnlyDictionary<string, string>
{
    public static ExternalTexts Load(string path) => new(path);

    private readonly ImmutableDictionary<string, string> _texts;

    public IEnumerable<string> Keys => _texts.Keys;
    public IEnumerable<string> Values => _texts.Values;
    public int Count => _texts.Count;

    public string this[string key] => _texts[key];

    internal ExternalTexts(string path)
    {
        Dictionary<string, string> texts = [];

        foreach (string line in File.ReadLines(path))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            int index = line.IndexOf('=');
            if (index < 0)
            {
                Debug.Log($"Invalid line in external texts: '{line}'.");
                continue;
            }
            string key = line[..index];
            string value = line[(index + 1)..];
            if (!texts.TryAdd(key, value))
                Debug.Log($"Duplicate key in external texts: '{key}'.");
        }

        _texts = texts.ToImmutableDictionary();
    }

    public bool ContainsKey(string key) => _texts.ContainsKey(key);
    public bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => _texts.TryGetValue(key, out value);
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _texts.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
