using System.Collections.Generic;
using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Sent by the server to display a notification dialog in client.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.NotificationDialog"/></item>
/// </list>
/// </summary>
/// <param name="type">The type of the notification dialog.</param>
public sealed class NotificationDialogMsg(string type) : Dictionary<string, string>, IMessage<NotificationDialogMsg>
{
    /// <summary>
    /// The type of the notification dialog.
    /// </summary>
    public string Type { get; set; } = type;

    static ClientType IMessage<NotificationDialogMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<NotificationDialogMsg>.Identifier => In.NotificationDialog;

    static NotificationDialogMsg IParser<NotificationDialogMsg>.Parse(in PacketReader p)
    {
        NotificationDialogMsg msg = new(p.ReadString());
        int n = p.ReadLength();
        for (int i = 0; i < n; i++)
            msg[p.ReadString()] = p.ReadString();
        return msg;
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteString(Type);
        p.WriteLength((Length)Count);
        foreach (var (key, value) in this)
        {
            p.WriteString(key);
            p.WriteString(value);
        }
    }
}