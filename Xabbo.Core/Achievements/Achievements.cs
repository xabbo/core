using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class Achievements : IAchievements, IReadOnlyCollection<Achievement>
    {
        private readonly ConcurrentDictionary<int, Achievement> _dict;

        public string DefaultCategory { get; set; }

        public int Count => _dict.Count;
        public IEnumerator<Achievement> GetEnumerator() => _dict.Select(x => x.Value).GetEnumerator();
        IEnumerator<IAchievement> IEnumerable<IAchievement>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Achievement? this[int id]
        {
            get => _dict.TryGetValue(id, out Achievement? ach) ? ach : null;
        }

        IAchievement? IAchievements.this[int id] => this[id];

        public Achievements()
        {
            _dict = new ConcurrentDictionary<int, Achievement>();

            DefaultCategory = string.Empty;
        }

        protected Achievements(IReadOnlyPacket packet)
            : this()
        {
            short n = packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
                Update(Achievement.Parse(packet));

            DefaultCategory = packet.ReadString();
        }

        public void Update(Achievement achievement)
        {
            _dict.AddOrUpdate(
                achievement.Id,
                achievement,
                (id, ach) => achievement
            );
        }

        public static Achievements Parse(IReadOnlyPacket packet) => new Achievements(packet);
    }
}
