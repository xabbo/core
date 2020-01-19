using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class UserSearchResults : IReadOnlyCollection<UserSearchResult>
    {
        public static UserSearchResults Parse(Packet packet) => new UserSearchResults(packet);

        public IReadOnlyList<UserSearchResult> Friends { get; }
        public IReadOnlyList<UserSearchResult> Others { get; }

        public int Count => Friends.Count + Others.Count;
        public IEnumerator<UserSearchResult> GetEnumerator() => Friends.Concat(Others).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal UserSearchResults(Packet packet)
        {
            var results = new List<UserSearchResult>();

            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                results.Add(UserSearchResult.Parse(packet));
            Friends = results.AsReadOnly();

            results = new List<UserSearchResult>();
            n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                results.Add(UserSearchResult.Parse(packet));
            Others = results.AsReadOnly();
        }
    }
}
