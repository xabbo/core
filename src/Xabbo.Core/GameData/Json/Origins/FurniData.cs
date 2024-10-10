using System;
using System.Collections.Generic;
using System.Text.Json;
using Xabbo.Core.Serialization;

namespace Xabbo.Core.GameData.Json.Origins;

public sealed class FurniData : List<FurniInfo>
{
    public static FurniData Load(string json)
    {
        string[] lines = json.Split("\n", StringSplitOptions.RemoveEmptyEntries);

        FurniData furniData = [];

        foreach (string line in lines)
        {
            // why do they have to have a literal tab character inside a json string...
            string fixedJson = line.Replace("\t", "\\t");
            var entries = JsonSerializer.Deserialize(fixedJson, OriginsGameDataJsonContext.Default.FurniData)
                ?? throw new Exception($"Failed to deserialize {nameof(FurniData)}.");

            furniData.AddRange(entries);
        }

        return furniData;
    }
}

public sealed class FurniInfo : List<string>
{
    private string GetString(int index, string defaultValue = "") =>
        (index < Count) ? this[index] : defaultValue;

    private int GetInt(int index, int defaultValue = 0) =>
        (index < Count && int.TryParse(this[index], out int value)) ? value : defaultValue;

    public ItemType Type => GetString(0) switch
    {
        "i" => ItemType.Wall,
        "s" => ItemType.Floor,
        _ => ItemType.None
    };
    public int Kind => GetInt(1);
    public string Identifier => GetString(2);
    public int Revision => GetInt(3);
    public int DefaultDir => GetInt(4);
    public int XDim => GetInt(5);
    public int YDim => GetInt(6);
    public string PartColors => GetString(7);
    public string Name => GetString(8).Trim();
    public string Description => GetString(9).Trim();
    public string AdUrl => GetString(10);
    public string CustomParams => GetString(15);
    public int SpecialType => GetInt(16);
}