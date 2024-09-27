using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a list of user search results.
/// </summary>
/// <remarks>
/// On Shockwave, this should contain a single user in the <see cref="Others"/> list.
/// </remarks>
public class UserSearchResults : IReadOnlyCollection<UserSearchResult>, IParserComposer<UserSearchResults>
{
    public IReadOnlyList<UserSearchResult> Friends { get; } = [];
    public IReadOnlyList<UserSearchResult> Others { get; } = [];

    public int Count => Friends.Count + Others.Count;
    public IEnumerator<UserSearchResult> GetEnumerator() => Friends.Concat(Others).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    protected UserSearchResults(in PacketReader p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            Others = [p.Parse<UserSearchResult>()];
        }
        else
        {
            Friends = [.. p.ParseArray<UserSearchResult>()];
            Others = [.. p.ParseArray<UserSearchResult>()];
        }
    }

    public UserSearchResult? GetResult(string name) => this.FirstOrDefault(result =>
        string.Equals(result.Name, name, StringComparison.OrdinalIgnoreCase));

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            if (Friends.Count > 0 || Others.Count != 1)
                throw new Exception("UserSearchResults must only have a single result in Others on Shockwave.");
            p.Compose(Others[0]);
        }
        else
        {
            p.ComposeArray(Friends);
            p.ComposeArray(Others);
        }
    }

    static UserSearchResults IParser<UserSearchResults>.Parse(in PacketReader p) => new(in p);
}
