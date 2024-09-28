using System;
using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when sending a message to a friend via the console.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.SendMsg"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.MESSENGER_SENDMSG"/></item>
/// </list>
/// </summary>
/// <remarks>
/// Only a single recipient is supported on <see cref="ClientType.Modern"/> clients.
/// On the <see cref="ClientType.Origins"/> client, you can specify multiple recipients.
/// </remarks>
public sealed class SendConsoleMessageMsg : IMessage<SendConsoleMessageMsg>
{
    public static Identifier Identifier => Out.SendMsg;

    /// <summary>
    /// The list of recipient IDs.
    /// Only a single recipient is supported on <see cref="ClientType.Modern"/> clients.
    /// </summary>
    public List<Id> Recipients { get; set; } = [];

    /// <summary>
    /// The message content.
    /// </summary>
    public string Message { get; set; } = "";

    public int ConfirmationId { get; set; }

    static SendConsoleMessageMsg IParser<SendConsoleMessageMsg>.Parse(in PacketReader p) => p.Client switch
    {
        ClientType.Shockwave => new()
        {
            Recipients = [.. p.ReadIdArray()],
            Message = p.ReadString()
        },
        not ClientType.Shockwave => new()
        {
            Recipients = [p.ReadId()],
            Message = p.ReadString(),
            ConfirmationId = p.ReadInt()
        }
    };

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            p.WriteIdArray(Recipients);
            p.WriteString(Message);
        }
        else
        {
            if (Recipients.Count != 1)
                throw new Exception($"Only a single recipient is supported on the {p.Client} client.");
            p.WriteId(Recipients[0]);
            p.WriteString(Message);
            p.WriteInt(ConfirmationId);
        }
    }
}
