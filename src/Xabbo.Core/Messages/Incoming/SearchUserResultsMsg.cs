using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record SearchUserResultsMsg(UserSearchResults Results) : IMessage<SearchUserResultsMsg>
{
    static Identifier IMessage<SearchUserResultsMsg>.Identifier => In.HabboSearchResult;
    static SearchUserResultsMsg IParser<SearchUserResultsMsg>.Parse(in PacketReader p) => new(p.Parse<UserSearchResults>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Results);
}
