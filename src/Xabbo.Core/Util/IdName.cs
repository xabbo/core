using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a name and ID.
/// </summary>
public sealed record IdName(Id Id, string Name) : IParserComposer<IdName>
{
    public static IdName Parse(in PacketReader p) => new(p.ReadId(), p.ReadString());
    public void Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteString(Name);
    }
}
