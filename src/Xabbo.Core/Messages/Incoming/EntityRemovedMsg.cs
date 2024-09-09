using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record EntityRemovedMsg(int Index) : IMessage<EntityRemovedMsg>
{
    static Identifier IMessage<EntityRemovedMsg>.Identifier => In.UserRemove;

    static EntityRemovedMsg IParser<EntityRemovedMsg>.Parse(in PacketReader p)
    {
        if (!int.TryParse(p.ReadString(), out int index))
            throw new FormatException("Failed to parse Index in EntityRemovedMsg.");
        return new(index);
    }

    void IComposer.Compose(in PacketWriter p) => p.WriteInt(Index);
}