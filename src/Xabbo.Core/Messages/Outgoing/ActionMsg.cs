using System;

using Xabbo.Messages;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when performing an action in a room.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Xabbo.Messages.Flash.Out.AvatarExpression"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.WAVE"/></item>
/// </list>
/// </summary>
/// <param name="Action">
/// The action to perform.
/// Only <see cref="AvatarAction.Wave"/> is supported on <see cref="ClientType.Origins"/>.
/// </param>
public sealed record ActionMsg(AvatarAction Action) : IMessage<ActionMsg>
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
        not ClientType.Shockwave => (AvatarAction)p.ReadInt(),
        ClientType.Shockwave => AvatarAction.Wave,
    });

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is not ClientType.Shockwave)
            p.WriteInt((int)Action);
        else if (Action is not AvatarAction.Wave)
            throw new NotSupportedException("Only the Wave action is supported on Shockwave.");
    }
}
