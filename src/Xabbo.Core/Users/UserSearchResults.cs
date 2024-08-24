using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;

namespace Xabbo.Core;

public class UserSearchResults : IReadOnlyCollection<UserSearchResult>, IComposer, IParser<UserSearchResults>
{
    public static UserSearchResults Parse(in PacketReader packet) => new(in packet);

    public IReadOnlyList<UserSearchResult> Friends { get; }
    public IReadOnlyList<UserSearchResult> Others { get; }

    public int Count => Friends.Count + Others.Count;
    public IEnumerator<UserSearchResult> GetEnumerator() => Friends.Concat(Others).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    protected UserSearchResults(in PacketReader p)
    {
        int n = p.Read<Length>();
        var results = new List<UserSearchResult>(n);
        for (int i = 0; i < n; i++)
            results.Add(p.Parse<UserSearchResult>());
        Friends = results.AsReadOnly();

        n = p.Read<Length>();
        results = new List<UserSearchResult>(n);
        for (int i = 0; i < n; i++)
            results.Add(p.Parse<UserSearchResult>());
        Others = results.AsReadOnly();
    }

    public UserSearchResult? GetResult(string name)
    {
        return this.FirstOrDefault(result =>
            string.Equals(result.Name, name, StringComparison.OrdinalIgnoreCase)
        );
    }

    public void Compose(in PacketWriter p)
    {
        p.Write(Friends);
        p.Write(Others);
    }
}
