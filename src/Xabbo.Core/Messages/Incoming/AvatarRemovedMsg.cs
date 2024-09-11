using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record AvatarRemovedMsg(int Index) : IMessage<AvatarRemovedMsg>
{
    static Identifier IMessage<AvatarRemovedMsg>.Identifier => In.UserRemove;

    static AvatarRemovedMsg IParser<AvatarRemovedMsg>.Parse(in PacketReader p)
    {
        if (!int.TryParse(p.ReadString(), out int index))
            throw new FormatException("Failed to parse Index in AvatarRemovedMsg.");
        return new(index);
    }

    void IComposer.Compose(in PacketWriter p) => p.WriteInt(Index);
}