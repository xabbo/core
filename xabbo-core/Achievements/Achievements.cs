﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class Achievements : IAchievements, IReadOnlyCollection<Achievement>
    {
        private readonly ConcurrentDictionary<int, Achievement> _dict;

        public static Achievements Parse(IReadOnlyPacket packet) => new Achievements(packet);

        public string String1 { get; set; }

        public int Count => _dict.Count;
        public IEnumerator<Achievement> GetEnumerator() => _dict.Values.GetEnumerator();
        IEnumerator<IAchievement> IEnumerable<IAchievement>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Achievement this[int id]
        {
            get => _dict.TryGetValue(id, out Achievement ach) ? ach : null;
        }

        IAchievement IAchievements.this[int id] => this[id];

        public Achievements()
        {
            _dict = new ConcurrentDictionary<int, Achievement>();
        }

        internal Achievements(IReadOnlyPacket packet)
            : this()
        {
            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
                Update(Achievement.Parse(packet));

            String1 = packet.ReadString();
        }

        public void Update(Achievement achievement)
        {
            _dict.AddOrUpdate(
                achievement.Id,
                achievement,
                (id, ach) => ach
            );
        }
    }
}
