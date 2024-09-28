using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when requesting a wall item's data.
/// <para/>
/// Request for <see cref="ItemDataMsg"/>. Returns a <see cref="string"/> representing the item's data.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.GetItemData"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.G_IDATA"/></item>
/// </list>
/// </summary>
public sealed record GetItemDataMsg(Id Id) : IRequestMessage<GetItemDataMsg, ItemDataMsg, string>
{
    static Identifier IMessage<GetItemDataMsg>.Identifier => Out.GetItemData;
    bool IRequestFor<ItemDataMsg>.MatchResponse(ItemDataMsg msg) => msg.Id == Id;
    string IResponseData<ItemDataMsg, string>.GetData(ItemDataMsg msg) => msg.Data;
    static GetItemDataMsg IParser<GetItemDataMsg>.Parse(in PacketReader p) => new(p.ReadId());
    void IComposer.Compose(in PacketWriter p) => p.WriteId(Id);
}
