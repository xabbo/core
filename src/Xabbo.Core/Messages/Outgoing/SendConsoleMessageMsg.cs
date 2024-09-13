using System;
using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed class SendConsoleMessageMsg : IMessage<SendConsoleMessageMsg>
{
    public static Identifier Identifier => Out.SendMsg;

    public List<Id> Recipients { get; set; } = [];
    public string Message { get; set; } = "";
    public int ConfirmationId { get; set; }

    public static SendConsoleMessageMsg Parse(in PacketReader p) => p.Client switch
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

    public void Compose(in PacketWriter p)
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