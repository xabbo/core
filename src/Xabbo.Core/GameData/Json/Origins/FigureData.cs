using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Xabbo.Core.Serialization;

namespace Xabbo.Core.GameData.Json.Origins;

public sealed class FigureData
{
    public static FigureData Load(string filePath)
    {
        byte[] data = File.ReadAllBytes(filePath);
        FixBuffer(data);

        return JsonSerializer.Deserialize(Encoding.ASCII.GetString(data), OriginsGameDataJsonContext.Default.FigureData)
            ?? throw new Exception("Failed to deserialize figure data.");
    }

    [JsonPropertyName("M")]
    public Dictionary<string, List<FigurePartSet>> MalePartSets { get; set; } = [];

    [JsonPropertyName("F")]
    public Dictionary<string, List<FigurePartSet>> FemalePartSets { get; set; } = [];

    public class FurniInfoContainer
    {
        public List<FurniInfo> FurniType { get; set; } = [];
    }

    public static void FixBuffer(byte[] buffer)
    {
        int sp = -1;
        Span<(int Index, bool IsObject)> stack = stackalloc (int, bool)[16];

        for (int i = 0; i < buffer.Length; i++)
        {
            // Assuming these characters won't appear inside any strings.
            switch (buffer[i])
            {
                case (byte)'[':
                    if (++sp >= stack.Length)
                        throw new Exception("Overflow in FigureData.FixBuffer.");
                    stack[sp] = (i, false);
                    break;
                case (byte)':':
                    stack[sp].IsObject = true;
                    break;
                case (byte)']':
                    if (sp < 0)
                        throw new Exception("Underflow in FigureData.FixBuffer.");
                    if (stack[sp].IsObject)
                    {
                        buffer[stack[sp].Index] = (byte)'{';
                        buffer[i] = (byte)'}';
                    }
                    sp--;
                    break;
            }
        }
    }
}

public sealed class FigurePartSet
{
    [JsonPropertyName("s")]
    public int Id { get; set; }

    [JsonPropertyName("p")]
    public Dictionary<string, int> Parts { get; set; } = [];

    [JsonPropertyName("c")]
    public List<string> Colors { get; set; } = [];
}
