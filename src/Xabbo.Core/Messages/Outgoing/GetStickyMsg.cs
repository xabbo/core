using Xabbo.Core.Messages.Incoming;
using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record GetStickyMsg(Id Id) : IRequestMessage<GetStickyMsg, ItemDataMsg, Sticky>
{
    static Identifier IMessage<GetStickyMsg>.Identifier => Out.GetItemData;
    bool IRequestFor<ItemDataMsg>.MatchResponse(ItemDataMsg msg) => msg.Id == Id;
    Sticky IResponseData<ItemDataMsg, Sticky>.GetData(ItemDataMsg msg) => Sticky.ParseString(msg.Id, msg.Data);
    static GetStickyMsg IParser<GetStickyMsg>.Parse(in PacketReader p) => new(p.ReadId());
    void IComposer.Compose(in PacketWriter p) => p.WriteId(Id);
}
