using Xabbo.Messages;

namespace Xabbo.Core;

public class FriendRequest : IParserComposer<FriendRequest>
{
    public Id Id { get; set; }
    public string Name { get; set; } = "";
    public string Figure { get; set; } = "";

    public FriendRequest() { }

    protected FriendRequest(in PacketReader p)
    {
        Id = p.ReadId();
        Name = p.ReadString();
        if (p.Client is not ClientType.Shockwave)
            Figure = p.ReadString();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteString(Name);
        if (p.Client is not ClientType.Shockwave)
            p.WriteString(Figure);
    }

    static FriendRequest IParser<FriendRequest>.Parse(in PacketReader p) => new(in p);
}
