using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when picking up an item from a room.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.PickupObject"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.ADDSTRIPITEM"/></item>
/// </list>
/// </summary>
/// <param name="Type">The type of the item to pick up.</param>
/// <param name="Id">The ID of the item to pick up.</param>
public record PickupItemMsg(ItemType Type, Id Id) : IMessage<PickupItemMsg>
{
    const int ExpectedFieldCount = 3;
    const string ExpectedField0 = "new";

    public static Identifier Identifier => Out.PickupObject;

    static PickupItemMsg IParser<PickupItemMsg>.Parse(in PacketReader p)
    {
        ItemType itemType = (ItemType)(-1);
        Id id = -1;

        switch (p.Client)
        {
            case not ClientType.Shockwave:
                int intType = p.ReadInt();
                itemType = intType switch
                {
                    1 => ItemType.Wall,
                    2 => ItemType.Floor,
                    _ => throw new Exception($"Unknown item type when parsing PickupItemMsg: {intType}.")
                };
                id = p.ReadId();
                break;
            case ClientType.Shockwave:
                string[] fields = p.ReadContent().Split();
                if (fields.Length != ExpectedFieldCount)
                    throw new Exception($"Unexpected field count when parsing PickupItemMsg: {fields.Length}. Expected: {ExpectedFieldCount}.");
                if (fields[0] != ExpectedField0)
                    throw new Exception($"Unexpected field when parsing PickupItemMsg: '{fields[0]}'. Expected: '{ExpectedField0}'.");
                itemType = fields[1] switch
                {
                    "item" => ItemType.Wall,
                    "stuff" => ItemType.Floor,
                    _ => throw new Exception($"Unexpected field when parsing PickupItemMsg: '{fields[1]}'."),
                };
                if (!Id.TryParse(fields[2], out id))
                    throw new FormatException($"Failed to parse Id in PickupItemMsg: '{fields[2]}'.");
                break;
        }

        return itemType switch
        {
            ItemType.Wall => new PickupItemMsg(ItemType.Floor, id),
            ItemType.Floor => new PickupItemMsg(ItemType.Wall, id),
            _ => throw new Exception("Unknown item type when parsing PickupItemMsg.")
        };
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client == ClientType.Shockwave)
        {
            string type = Type switch
            {
                ItemType.Wall => "item",
                ItemType.Floor => "stuff",
                _ => throw new Exception("Unknown item type when composing PickupItemMsg.")
            };
            p.WriteContent($"new {type} {Id}");
        }
        else
        {
            p.WriteInt(Type switch
            {
                ItemType.Wall => 1,
                ItemType.Floor => 2,
                _ => throw new Exception("Unknown item type when composing PickupItemMsg.")
            });
            p.WriteId(Id);
        }
    }
}
