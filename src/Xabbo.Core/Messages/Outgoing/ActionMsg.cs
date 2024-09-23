using System;

using Xabbo.Messages;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record ActionMsg(Actions Action = Actions.None) : IMessage<ActionMsg>
{
    static Identifier IMessage<ActionMsg>.Identifier => default;
    static Identifier[] IMessage<ActionMsg>.Identifiers => [
        Xabbo.Messages.Flash.Out.AvatarExpression,
        Xabbo.Messages.Shockwave.Out.WAVE,
    ];
    static bool IMessage<ActionMsg>.UseTargetedIdentifiers => true;

    Identifier IMessage.GetIdentifier(ClientType client) => client switch
    {
        not ClientType.Shockwave => Xabbo.Messages.Flash.Out.AvatarExpression,
        ClientType.Shockwave => Xabbo.Messages.Shockwave.Out.WAVE,
    };

    static ActionMsg IParser<ActionMsg>.Parse(in PacketReader p) => new(p.Client switch
    {
        not ClientType.Shockwave => (Actions)p.ReadInt(),
        ClientType.Shockwave => Actions.Wave,
    });

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is not ClientType.Shockwave)
            p.WriteInt((int)Action);
        else if (Action is not Actions.Wave)
            throw new NotSupportedException("Only the Wave action is supported on Shockwave.");
    }
}
