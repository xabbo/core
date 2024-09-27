using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IAchievements"/>
public sealed class Achievements : IAchievements, ICollection<Achievement>, IParserComposer<Achievements>
{
    private readonly ConcurrentDictionary<int, Achievement> _dict = [];
    public string DefaultCategory { get; set; } = "";
    public int Count => _dict.Count;
    public bool IsReadOnly => false;

    public IEnumerator<Achievement> GetEnumerator() => _dict.Select(x => x.Value).GetEnumerator();
    IEnumerator<IAchievement> IEnumerable<IAchievement>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public Achievement? this[int id]
    {
        get => _dict.TryGetValue(id, out Achievement? ach) ? ach : null;
    }

    IAchievement? IAchievements.this[int id] => this[id];

    public Achievements() { }

    private Achievements(in PacketReader p) : this()
    {
        UnsupportedClientException.ThrowIfOrigins(p.Client);

        foreach (var achievement in p.ParseArray<Achievement>())
            Add(achievement);

        DefaultCategory = p.ReadString();
    }

    public void Add(Achievement achievement) => _dict.AddOrUpdate(
        achievement.Id, achievement,
        (id, ach) => achievement
    );

    public void Clear() => _dict.Clear();

    public bool Contains(Achievement item) => _dict.ContainsKey(item.Id);

    public bool Remove(Achievement item) => _dict.Remove(item.Id, out _);

    void ICollection<Achievement>.CopyTo(Achievement[] array, int arrayIndex) => _dict.Values.CopyTo(array, arrayIndex);

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfOrigins(p.Client);

        p.ComposeArray(_dict.Values);
        p.WriteString(DefaultCategory);
    }

    static Achievements IParser<Achievements>.Parse(in PacketReader p) => new(in p);
}
