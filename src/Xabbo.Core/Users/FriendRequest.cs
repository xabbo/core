using Xabbo.Messages;

namespace Xabbo.Core;

public class FriendRequest : IParserComposer<FriendRequest>
{
    public Id Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Figure { get; set; } = string.Empty;

    public FriendRequest() { }

    protected FriendRequest(in PacketReader p)
    {
        Id = p.ReadId();
        Name = p.ReadString();
        Figure = p.ReadString();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteString(Name);
        p.WriteString(Figure);
    }

    static FriendRequest IParser<FriendRequest>.Parse(in PacketReader p) => new(in p);
}
