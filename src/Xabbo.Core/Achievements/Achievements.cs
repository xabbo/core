using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class Achievements : IAchievements, IReadOnlyCollection<Achievement>, IComposer, IParser<Achievements>
{
    private readonly ConcurrentDictionary<int, Achievement> _dict = [];

    public string DefaultCategory { get; set; } = "";

    public int Count => _dict.Count;
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
        int n = p.Read<Length>();
        for (int i = 0; i < n; i++)
            Update(Achievement.Parse(in p));

        DefaultCategory = p.Read<string>();
    }

    public void Update(Achievement achievement)
    {
        _dict.AddOrUpdate(
            achievement.Id,
            achievement,
            (id, ach) => achievement
        );
    }

    public void Compose(in PacketWriter p)
    {
        p.Write<Length>(Count);
        foreach (var achievement in _dict.Values)
            p.Write(achievement);
        p.Write(DefaultCategory);
    }

    public static Achievements Parse(in PacketReader p) => new(in p);
}
