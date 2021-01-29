using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class UserSearchResults : IReadOnlyCollection<UserSearchResult>
    {
        public static UserSearchResults Parse(IReadOnlyPacket packet) => new UserSearchResults(packet);

        public IReadOnlyList<UserSearchResult> Friends { get; }
        public IReadOnlyList<UserSearchResult> Others { get; }

        public int Count => Friends.Count + Others.Count;
        public IEnumerator<UserSearchResult> GetEnumerator() => Friends.Concat(Others).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected UserSearchResults(IReadOnlyPacket packet)
        {
            var results = new List<UserSearchResult>();

            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
                results.Add(UserSearchResult.Parse(packet));
            Friends = results.AsReadOnly();

            results = new List<UserSearchResult>();
            n = packet.ReadInt();
            for (int i = 0; i < n; i++)
                results.Add(UserSearchResult.Parse(packet));
            Others = results.AsReadOnly();
        }

        public UserSearchResult? GetResult(string name)
        {
            return this.FirstOrDefault(result =>
                string.Equals(result.Name, name, StringComparison.OrdinalIgnoreCase)
            );
        }
    }
}
