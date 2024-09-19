using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing.Modern;

/// <summary>
/// Sent when updating the user's avatar looks.
/// <para/>
/// Supported clients:
/// <list type="bullet">
/// <item>Flash (Incoming, Outgoing)</item>
/// <item>Shockwave (Outgoing -> translates to <see cref="Origins.UpdateProfileMsg"/>)</item>
/// </list>
/// </summary>
public sealed record UpdateAvatarMsg(Gender Gender, string Figure) : IMessage<UpdateAvatarMsg>
{
    static Identifier IMessage<UpdateAvatarMsg>.Identifier => Out.UpdateFigureData; // TODO check header

    Identifier IMessage.GetIdentifier(ClientType client) => client switch
    {
        ClientType.Shockwave => Xabbo.Messages.Shockwave.Out.UPDATE,
        not ClientType.Shockwave => Out.UpdateFigureData
    };

    public UpdateAvatarMsg(Figure figure) : this(figure.Gender, figure.ToString()) { }

    static UpdateAvatarMsg IParser<UpdateAvatarMsg>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfOrigins(p.Client);

        return new(
            Gender: H.ToGender(p.ReadString()),
            Figure: p.ReadString()
        );
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            p.Compose(new Origins.UpdateProfileMsg
            {
                Gender = Gender.ToClientString(),
                Figure = Figure
            });
        }
        else
        {
            p.WriteString(Gender.ToClientString());
            p.WriteString(Figure);
        }
    }
}
