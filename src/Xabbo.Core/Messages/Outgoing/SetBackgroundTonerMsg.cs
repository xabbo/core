using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when updating a room background toner.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.SetRoomBackgroundColorData"/></item>
/// </list>
/// </summary>
/// <param name="Id">The ID of the background toner.</param>
/// <param name="Hue">The hue of the background toner, ranging from 0-255.</param>
/// <param name="Saturation">The saturation of the background toner, ranging from 0-255.</param>
/// <param name="Value">The value of the background toner, ranging from 0-255.</param>
public sealed record SetBackgroundTonerColor(Id Id, int Hue, int Saturation, int Value) : IMessage<SetBackgroundTonerColor>
{
    static ClientType IMessage<SetBackgroundTonerColor>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<SetBackgroundTonerColor>.Identifier => Out.SetRoomBackgroundColorData;

    static SetBackgroundTonerColor IParser<SetBackgroundTonerColor>.Parse(in PacketReader p) => new(
        Id: p.ReadId(),
        Hue: p.ReadInt(),
        Saturation: p.ReadInt(),
        Value: p.ReadInt()
    );

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteInt(Hue);
        p.WriteInt(Saturation);
        p.WriteInt(Value);
    }
}
