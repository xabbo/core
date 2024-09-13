using Xabbo.Messages;

namespace Xabbo.Core;

public sealed record FriendCategory(int Id, string Name) : IParserComposer<FriendCategory>
{
    static FriendCategory IParser<FriendCategory>.Parse(in PacketReader p) => new(p.ReadInt(), p.ReadString());
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Id);
        p.WriteString(Name);
    }
}