using Xabbo.Core.Messages.Incoming;
using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when requesting a sticky note's contents.
/// <para/>
/// Request for <see cref="ItemDataMsg"/>. Returns a <see cref="Sticky"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.GetItemData"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.G_IDATA"/></item>
/// </list>
/// </summary>
/// <param name="Id">The ID of the sticky note to retrieve.</param>
public sealed record GetStickyMsg(Id Id) : IRequestMessage<GetStickyMsg, ItemDataMsg, Sticky>
{
    static Identifier IMessage<GetStickyMsg>.Identifier => Out.GetItemData;
    bool IRequestFor<ItemDataMsg>.MatchResponse(ItemDataMsg msg) => msg.Id == Id;
    Sticky IResponseData<ItemDataMsg, Sticky>.GetData(ItemDataMsg msg) => Sticky.ParseString(msg.Id, msg.Data);
    static GetStickyMsg IParser<GetStickyMsg>.Parse(in PacketReader p) => new(p.ReadId());
    void IComposer.Compose(in PacketWriter p) => p.WriteId(Id);
}
