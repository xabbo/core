using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    public class EntityStatusUpdate : IEntityStatusUpdate, IReadOnlyDictionary<string, IReadOnlyList<string>>
    {
        private readonly Dictionary<string, string[]> fragments
            = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        public int Index { get; set; }
        public Tile Location { get; set; }
        public int HeadDirection { get; set; }
        public int Direction { get; set; }
        public string Status
        {
            get => CompileStatus();
            set => ParseStatus(value);
        }

        // sit, lay
        public Stances Stance
        {
            get
            {
                if (fragments.ContainsKey("sit"))
                    return Stances.Sit;
                else if (fragments.ContainsKey("lay"))
                    return Stances.Lay;
                else
                    return Stances.Stand;
            }

            set
            {
                switch (value)
                {
                    case Stances.Stand:
                        fragments.Remove("sit");
                        fragments.Remove("lay");
                        break;
                    case Stances.Sit:
                        if (!fragments.ContainsKey("sit"))
                            fragments["sit"] = new string[] { "0.0", "0" };
                        fragments.Remove("lay");
                        break;
                    case Stances.Lay:
                        if (!fragments.ContainsKey("lay"))
                            fragments["lay"] = new string[] { "0.0", "0" };
                        fragments.Remove("sit");
                        break;
                    default: break;
                }
            }
        }

        // flatctrl
        public bool IsController => fragments.ContainsKey("flatctrl");
        public int ControlLevel
        {
            get => IsController ? int.Parse(fragments["flatctrl"][0]) : 0;
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
                {
                    if (!fragments.ContainsKey("trd"))
                        fragments.Add("trd", new string[0]);
                }
                else
                {
                    fragments.Remove("trd");
                }
            }
        }

        // mv
        public Tile? MovingTo
        {
            get => fragments.TryGetValue("mv", out string[]? args) ? Tile.Parse(args[0]) : (Tile?)null;
            set
            {
                if (value == null)
                {
                    fragments.Remove("mv");
                }
                else
                {
                    fragments["mv"] = new string[] { value.ToString() ?? string.Empty };
                }
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
                    if (args.Length < 2)
                    {
                        // TODO
                    }
                    args[1] = value ? "1" : "0";
                }
                else
                {
                    Stance = Stances.Sit;
                    fragments["sit"] = args = new string[] { "0.0", value ? "1" : "0" };
                }
            }
        }

        // sit, lay
        public double? ActionHeight
        {
            get
            {
                switch (Stance)
                {
                    case Stances.Sit:
                    case Stances.Lay:
                        if (fragments.TryGetValue(Stance.ToString(), out string[]? args))
                        {
                            if (args.Length > 0)
                                return double.Parse(args[0]);
                        }
                        break;
                    default:
                        break;
                }

                return null;
            }

            set
            {
                switch (Stance)
                {
                    case Stances.Sit:
                    case Stances.Lay:
                        if (!value.HasValue)
                            throw new ArgumentNullException("ActionHeight", "Action height cannot be null");
                        fragments[Stance.ToString()][0] = value.Value.ToString("0.0###############");
                        break;
                    default: throw new InvalidOperationException($"Cannot set action height for stance: {Stance}");
                }
            }
        }

        // sign
        public Signs Sign
        {
            get
            {
                return fragments.ContainsKey("sign") ? (Signs)int.Parse(fragments["sign"][0]) : Signs.None;
            }

            set
            {
                if (value == Signs.None)
                    fragments.Remove("sign");
                else
                    fragments["sign"] = new string[] { ((int)Sign).ToString() };
            }
        }

        IEnumerable<string> IReadOnlyDictionary<string, IReadOnlyList<string>>.Keys => fragments.Keys;
        IEnumerable<IReadOnlyList<string>> IReadOnlyDictionary<string, IReadOnlyList<string>>.Values => fragments.Values;
        int IReadOnlyCollection<KeyValuePair<string, IReadOnlyList<string>>>.Count => fragments.Count;

        public IReadOnlyList<string> this[string key] => fragments[key];

        public EntityStatusUpdate()
        {
            Location = Tile.Zero;
            HeadDirection = 0;
            Direction = 0;
        }

        private EntityStatusUpdate(IReadOnlyPacket packet)
        {
            Index = packet.ReadInt();
            Location = Tile.Parse(packet);
            HeadDirection = packet.ReadInt();
            Direction = packet.ReadInt();

            ParseStatus(packet.ReadString());
        }

        public void Write(IPacket packet)
        {
            packet.WriteInt(Index);
            Location.Write(packet);
            packet.WriteInt(HeadDirection);
            packet.WriteInt(Direction);
            packet.WriteString(CompileStatus());
        }

        private void ParseStatus(string status)
        {
            fragments.Clear();

            string[] parts = status.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                string type;
                string[] args = new string[0];

                int spaceIndex = part.IndexOf(' ');
                if (spaceIndex > 0)
                {
                    type = part.Substring(0, spaceIndex);
                    args = part
                        .Substring(spaceIndex + 1)
                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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
            foreach (var pair in fragments)
            {
                if (sb.Length > 0) sb.Append('/');
                sb.Append(pair.Key);
                for (int i = 0; i < pair.Value.Length; i++)
                {
                    sb.Append(' ');
                    sb.Append(pair.Value[i]);
                }
            }
            return sb.ToString();
        }

        bool IReadOnlyDictionary<string, IReadOnlyList<string>>.ContainsKey(string key) => fragments.ContainsKey(key);
        bool IReadOnlyDictionary<string, IReadOnlyList<string>>.TryGetValue(string key, out IReadOnlyList<string> value)
        {
            if (fragments.TryGetValue(key, out string[] array))
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
            => ((IEnumerable<KeyValuePair<string, IReadOnlyCollection<string>>>)this).GetEnumerator();

        public static EntityStatusUpdate Parse(IReadOnlyPacket packet) => new EntityStatusUpdate(packet);
        public static EntityStatusUpdate[] ParseAll(IReadOnlyPacket packet)
        {
            int n = packet.ReadShort();
            EntityStatusUpdate[] entityUpdates = new EntityStatusUpdate[n];
            for (int i = 0; i < n; i++)
                entityUpdates[i] = Parse(packet);
            return entityUpdates;
        }

        public static void WriteAll(IPacket packet, IEnumerable<IEntityStatusUpdate> updates)
        {
            packet.WriteShort((short)updates.Count());
            foreach (var update in updates)
                update.Write(packet);
        }
    }
}
