using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IAvatarStatus"/>
public class AvatarStatus : IAvatarStatus, IReadOnlyDictionary<string, IReadOnlyList<string>>, IParserComposer<AvatarStatus>
{
    private readonly Dictionary<string, string[]> fragments = new(StringComparer.OrdinalIgnoreCase);
    private static readonly char[] separator = ['/'];

    public int Index { get; set; }
    public Tile Location { get; set; }
    public int HeadDirection { get; set; }
    public int Direction { get; set; }

    // sit, lay
    public AvatarStance Stance
    {
        get
        {
            if (fragments.ContainsKey("sit"))
                return AvatarStance.Sit;
            else if (fragments.ContainsKey("lay"))
                return AvatarStance.Lay;
            else
                return AvatarStance.Stand;
        }

        set
        {
            switch (value)
            {
                case AvatarStance.Stand:
                    fragments.Remove("sit");
                    fragments.Remove("lay");
                    break;
                case AvatarStance.Sit:
                    if (!fragments.ContainsKey("sit"))
                        fragments["sit"] = ["0.0", "0"];
                    fragments.Remove("lay");
                    break;
                case AvatarStance.Lay:
                    if (!fragments.ContainsKey("lay"))
                        fragments["lay"] = ["0.0", "0"];
                    fragments.Remove("sit");
                    break;
                default: break;
            }
        }
    }

    // flatctrl
    public bool IsController => fragments.ContainsKey("flatctrl");
    public RightsLevel RightsLevel
    {
        get => IsController ? (RightsLevel)int.Parse(fragments["flatctrl"][0]) : 0;
        set
        {
            if (!fragments.ContainsKey("flatctrl"))
                fragments["flatctrl"] = new string[1];
            fragments["flatctrl"][0] = value.ToString();
        }
    }

    // trd
    public bool IsTrading
    {
        get => fragments.ContainsKey("trd");
        set
        {
            if (value)
                fragments.TryAdd("trd", []);
            else
                fragments.Remove("trd");
        }
    }

    // mv
    public Tile? MovingTo
    {
        get => fragments.TryGetValue("mv", out string[]? args) ? Tile.ParseString(args[0]) : (Tile?)null;
        set
        {
            if (value is { } tile)
                fragments["mv"] = [$"{tile.X},{tile.Y},{(FloatString)tile.Z}"];
            else
                fragments.Remove("mv");
        }
    }

    // sit
    public bool SittingOnFloor
    {
        get
        {
            if (!fragments.TryGetValue("sit", out string[]? args))
                return false;
            return args.Length > 1 && args[1] == "1";
        }

        set
        {
            if (fragments.TryGetValue("sit", out string[]? args))
            {
                if (args.Length == 0)
                {
                    fragments["sit"] = ["0.0", value ? "1" : "0"];
                }
                else if (args.Length == 1)
                {
                    fragments["sit"] = [args[0], value ? "1" : "0"];
                }
                else
                {
                    args[1] = value ? "1" : "0";
                }
            }
            else
            {
                Stance = AvatarStance.Sit;
                fragments["sit"] = ["0.0", value ? "1" : "0"];
            }
        }
    }

    // sit, lay
    public float? StanceHeight
    {
        get
        {
            if (Stance is AvatarStance.Sit or AvatarStance.Lay)
            {
                if (fragments.TryGetValue(Stance.ToString(), out string[]? args))
                {
                    if (args.Length > 0)
                        return float.Parse(args[0]);
                }
            }
            return null;
        }

        set
        {
            if (Stance is AvatarStance.Sit or AvatarStance.Lay)
            {
                if (value is not { } height)
                    throw new ArgumentNullException(nameof(StanceHeight), "Stance height cannot be set to null.");
                fragments[Stance.ToString()][0] = FloatString.Format(height);
            }
            else
            {
                throw new InvalidOperationException($"Cannot set action height for stance: {Stance}.");
            }
        }
    }

    // sign
    public AvatarSign Sign
    {
        get => fragments.TryGetValue("sign", out var value)
            ? (AvatarSign)int.Parse(value[0])
            : AvatarSign.None;

        set
        {
            if (value is AvatarSign.None)
                fragments.Remove("sign");
            else
                fragments["sign"] = [((int)value).ToString()];
        }
    }

    IEnumerable<string> IReadOnlyDictionary<string, IReadOnlyList<string>>.Keys => fragments.Keys;
    IEnumerable<IReadOnlyList<string>> IReadOnlyDictionary<string, IReadOnlyList<string>>.Values => fragments.Values;
    int IReadOnlyCollection<KeyValuePair<string, IReadOnlyList<string>>>.Count => fragments.Count;

    public IReadOnlyList<string> this[string key] => fragments[key];

    public AvatarStatus()
    {
        Location = default;
        HeadDirection = 0;
        Direction = 0;
    }

    public AvatarStatus(IAvatarStatus original)
    {
        Index = original.Index;
        Location = original.Location;
        HeadDirection = original.HeadDirection;
        Direction = original.Direction;
        ParseStatus(original.ToString() ?? "");
    }

    private AvatarStatus(in PacketReader p)
    {
        Index = p.ReadInt();
        Location = p.Parse<Tile>();
        HeadDirection = p.ReadInt();
        Direction = p.ReadInt();

        ParseStatus(p.ReadString());
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Index);
        p.Compose(Location);
        p.WriteInt(HeadDirection);
        p.WriteInt(Direction);
        p.WriteString(CompileStatus());
    }

    private void ParseStatus(string status)
    {
        fragments.Clear();

        string[] parts = status.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        foreach (string part in parts)
        {
            string type;
            string[] args = [];

            int spaceIndex = part.IndexOf(' ');
            if (spaceIndex > 0)
            {
                type = part[..spaceIndex];
                args = part[(spaceIndex + 1)..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                type = part;
            }

            fragments[type] = args;
        }
    }

    public string CompileStatus()
    {
        var sb = new StringBuilder();
        foreach (var (k, v) in fragments)
        {
            sb.Append('/');
            sb.Append(k);
            for (int i = 0; i < v.Length; i++)
            {
                sb.Append(' ');
                sb.Append(v[i]);
            }
        }
        sb.Append('/');
        return sb.ToString();
    }

    bool IReadOnlyDictionary<string, IReadOnlyList<string>>.ContainsKey(string key) => fragments.ContainsKey(key);
    bool IReadOnlyDictionary<string, IReadOnlyList<string>>.TryGetValue(string key,
        [NotNullWhen(true)] out IReadOnlyList<string>? value)
    {
        if (fragments.TryGetValue(key, out string[]? array))
        {
            value = array;
            return true;
        }
        else
        {
            value = null;
            return false;
        }
    }

    IEnumerator<KeyValuePair<string, IReadOnlyList<string>>>
        IEnumerable<KeyValuePair<string, IReadOnlyList<string>>>.GetEnumerator()
    {
        return fragments
            .Select(x => new KeyValuePair<string, IReadOnlyList<string>>(x.Key, x.Value))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, IReadOnlyCollection<string>>>)this).GetEnumerator();
    }

    public override string ToString() => CompileStatus();

    static AvatarStatus IParser<AvatarStatus>.Parse(in PacketReader p) => new(in p);
}
