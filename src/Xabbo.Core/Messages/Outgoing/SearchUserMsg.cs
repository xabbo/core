using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record SearchUserMsg(string Name, string? Type = null)
    : IRequestMessage<SearchUserMsg, SearchUserResultsMsg, UserSearchResults>
{
    static Identifier IMessage<SearchUserMsg>.Identifier => Out.HabboSearch;

    UserSearchResults IResponseData<SearchUserResultsMsg, UserSearchResults>.GetData(SearchUserResultsMsg msg) => msg.Results;

    static SearchUserMsg IParser<SearchUserMsg>.Parse(in PacketReader p) => p.Client switch
    {
        ClientType.Shockwave => new(Name: p.ReadString(), Type: p.ReadString()),
        not ClientType.Shockwave => new(p.ReadString())
    };

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteString(Name);
        if (p.Client is ClientType.Shockwave)
            p.WriteString(Type ?? "MESSENGER");
    }
}
