using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when a floor item's data is updated.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.ObjectDataUpdate"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.STUFFDATAUPDATE"/></item>
/// </list>
/// </summary>
/// <param name="Id">The ID of the floor item that was updated.</param>
/// <param name="Data">The updated item data.</param>
public sealed record FloorItemDataUpdatedMsg(Id Id, ItemData Data) : IMessage<FloorItemDataUpdatedMsg>
{
    static Identifier IMessage<FloorItemDataUpdatedMsg>.Identifier => In.ObjectDataUpdate;

    static FloorItemDataUpdatedMsg IParser<FloorItemDataUpdatedMsg>.Parse(in PacketReader p) => new(
        p.Client switch
        {
            ClientType.Unity => p.ReadId(),
            not ClientType.Unity => p.ReadString() switch
            {
                string s when Id.TryParse(s, out Id id) => id,
                string s => throw new FormatException($"Failed to parse {nameof(Id)} in {nameof(FloorItemDataUpdatedMsg)}.")
            },
        },
        p.Parse<ItemData>()
    );

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Unity)
            p.WriteId(Id);
        else
            p.WriteString(Id.ToString());

        p.Compose(Data);
    }
}