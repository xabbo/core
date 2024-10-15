using Xabbo.Messages;

namespace Xabbo.Core;

public class ConsoleMessage : IParserComposer<ConsoleMessage>
{
    public Id ChatId { get; set; }
    public string Content { get; set; } = "";
    public int SecondsSinceSent { get; set; }
    public string? Time { get; set; }
    public string MessageId { get; set; } = "";
    public int ConfirmationId { get; set; }
    public Id SenderId { get; set; }
    public string? SenderName { get; set; }
    public string SenderFigure { get; set; } = "";

    static ConsoleMessage IParser<ConsoleMessage>.Parse(in PacketReader p) => p.Client switch
    {
        ClientType.Shockwave => new()
        {
            MessageId = p.ReadString(),
            SenderId = p.ReadId(),
            Time = p.ReadString(),
            Content = p.ReadString().Replace('\r', '\n'),
        },
        not ClientType.Shockwave => new()
        {
            ChatId = p.ReadId(),
            Content = p.ReadString(),
            SecondsSinceSent = p.ReadInt(),
            MessageId = p.ReadString(),
            ConfirmationId = p.ReadInt(),
            SenderId = p.ReadId(),
            SenderName = p.ReadString(),
            SenderFigure = p.ReadString(),
        }
    };

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            p.WriteString(MessageId);
            p.WriteId(SenderId);
            p.WriteString(Time ?? "");
            p.WriteString(Content.Replace('\n', '\r'));
        }
        else
        {
            p.WriteId(ChatId);
            p.WriteString(Content);
            p.WriteInt(SecondsSinceSent);
            p.WriteString(MessageId);
            p.WriteInt(ConfirmationId);
            p.WriteId(SenderId);
            p.WriteString(SenderName ?? "");
            p.WriteString(SenderFigure);
        }
    }
}
