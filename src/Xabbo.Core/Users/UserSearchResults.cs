using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;

namespace Xabbo.Core;

public class UserSearchResults : IReadOnlyCollection<UserSearchResult>, IParserComposer<UserSearchResults>
{
    public IReadOnlyList<UserSearchResult> Friends { get; }
    public IReadOnlyList<UserSearchResult> Others { get; }

    public int Count => Friends.Count + Others.Count;
    public IEnumerator<UserSearchResult> GetEnumerator() => Friends.Concat(Others).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    protected UserSearchResults(in PacketReader p)
    {
        Friends = [..p.ParseArray<UserSearchResult>()];
        Others = [..p.ParseArray<UserSearchResult>()];
    }

    public UserSearchResult? GetResult(string name) => this.FirstOrDefault(result =>
        string.Equals(result.Name, name, StringComparison.OrdinalIgnoreCase));

    void IComposer.Compose(in PacketWriter p)
    {
        p.ComposeArray(Friends);
        p.ComposeArray(Others);
    }

    static UserSearchResults IParser<UserSearchResults>.Parse(in PacketReader p) => new(in p);
}
