using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when kicking a user from the room.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.KickUser"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.KICKUSER"/></item>
/// </list>
/// </summary>
/// <param name="Id">The ID of the user to kick. Applies to <see cref="ClientType.Modern"/> clients.</param>
/// <param name="Name">The name of the user to kick. Applies to the <see cref="ClientType.Origins"/> client.</param>
public sealed record KickUserMsg(Id? Id, string? Name) : IMessage<KickUserMsg>
{
    static Identifier IMessage<KickUserMsg>.Identifier => Out.KickUser;

    static KickUserMsg IParser<KickUserMsg>.Parse(in PacketReader p) => p.Client switch
    {
        ClientType.Shockwave => new KickUserMsg(null, p.ReadString()),
        not ClientType.Shockwave => new KickUserMsg(p.ReadId(), null)
    };

    /// <summary>
    /// Constructs a new <see cref="KickUserMsg"/> with the specified user.
    /// </summary>
    /// <param name="user">The user to kick.</param>
    public KickUserMsg(IUser user) : this(user.Id, user.Name) { }

    /// <summary>
    /// Constructs a new <see cref="KickUserMsg"/> with the specified user ID and name.
    /// </summary>
    /// <param name="user">The ID and name of the user to kick.</param>
    public KickUserMsg(IdName user) : this(user.Id, user.Name) { }

    /// <summary>
    /// Constructs a new <see cref="KickUserMsg"/> with the specified user ID.
    /// Applies to <see cref="ClientType.Modern"/> clients.
    /// </summary>
    /// <param name="id">The ID of the user to kick.</param>
    public KickUserMsg(Id id) : this(id, null) { }

    /// <summary>
    /// Constructs a new <see cref="KickUserMsg"/> with the specified name.
    /// Applies to the <see cref="ClientType.Origins"/> client.
    /// </summary>
    /// <param name="name">The name of the user to kick.</param>
    public KickUserMsg(string name) : this(null, name) { }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            if (Name is null)
                throw new Exception($"{nameof(KickUserMsg)} requires the user's name on {p.Client}.");
            p.WriteString(Name);
        }
        else
        {
            if (Id is not { } id)
                throw new Exception($"{nameof(KickUserMsg)} requires the user's ID on {p.Client}.");
            p.WriteId(id);
        }
    }
}
