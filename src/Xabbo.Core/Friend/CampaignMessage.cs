using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class CampaignMessage : IParserComposer<CampaignMessage>
{
    public int Id { get; set; }
    public string Url { get; set; } = "";
    public string Link { get; set; } = "";
    public string Message { get; set; } = "";

    static CampaignMessage IParser<CampaignMessage>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ~ClientType.Shockwave);

        return new()
        {
            Id = p.ReadInt(),
            Url = p.ReadString(),
            Link = p.ReadString(),
            Message = p.ReadString(),
        };
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ~ClientType.Shockwave);

        p.WriteInt(Id);
        p.WriteString(Url);
        p.WriteString(Link);
        p.WriteString(Message);
    }
}