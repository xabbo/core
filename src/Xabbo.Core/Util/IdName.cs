using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a name and ID.
/// </summary>
public sealed record IdName(Id Id, string Name) : IParserComposer<IdName>
{
    static IdName IParser<IdName>.Parse(in PacketReader p) => new(p.ReadId(), p.ReadString());
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteString(Name);
    }

    public static implicit operator IdName((Id id, string name) idName) => new(idName.id, idName.name);
}
