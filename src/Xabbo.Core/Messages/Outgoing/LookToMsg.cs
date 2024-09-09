using System;
using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record LookToMsg(int X, int Y) : IMessage<LookToMsg>
{
    public static Identifier Identifier => Out.LookTo;

    public static LookToMsg Parse(in PacketReader p)
    {
        int x, y;
        if (p.Client is ClientType.Shockwave)
        {
            string[] fields = p.Content.Split(' ');
            if (fields.Length != 2)
                throw new Exception($"Unexpected field count when parsing LookToMsg: {fields.Length}.");
            if (!int.TryParse(fields[0], out x))
                throw new Exception($"Failed to parse X in LookToMsg.");
            if (!int.TryParse(fields[1], out y))
                throw new Exception($"Failed to parse Y in LookToMsg.");
        }
        else
        {
            x = p.ReadInt();
            y = p.ReadInt();
        }

        return new LookToMsg(x, y);
    }

    public void Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            p.Content = $"{X} {Y}";
        }
        else
        {
            p.WriteInt(X);
            p.WriteInt(Y);
        }
    }

}