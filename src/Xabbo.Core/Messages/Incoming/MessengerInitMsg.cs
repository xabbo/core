using System.Collections.Generic;
using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when initializing the messenger.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>.
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.MessengerInit"/>.</item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.FRIEND_LIST_INIT"/>.</item>
/// </list>
/// </summary>
public sealed record MessengerInitMsg : IMessage<MessengerInitMsg>
{
    public static Identifier Identifier => In.MessengerInit;

    public string PersistentMessage { get; init; } = "";
    public int UserLimit { get; init; }
    public int NormalLimit { get; init; }
    public int ExtendedLimit { get; init; }
    public List<Friend> Friends { get; init; } = [];
    public List<ConsoleMessage> Messages { get; init; } = [];
    public List<CampaignMessage> CampaignMessages { get; init; } = [];
    public List<FriendRequest> Requests { get; init; } = [];
    public List<(int Id, string Name)> Categories { get; init; } = [];

    public static MessengerInitMsg Parse(in PacketReader p)
    {
        if (p.Client is not ClientType.Shockwave)
        {
            MessengerInitMsg msg = new()
            {
                UserLimit = p.ReadInt(),
                NormalLimit = p.ReadInt(),
                ExtendedLimit = p.ReadInt(),
            };
            int n = p.ReadLength();
            for (int i = 0; i < n; i++)
                msg.Categories.Add((p.ReadInt(), p.ReadString()));
            return msg;
        }
        else
        {
            return new MessengerInitMsg
            {
                PersistentMessage = p.ReadString(),
                UserLimit = p.ReadInt(),
                NormalLimit = p.ReadInt(),
                ExtendedLimit = p.ReadInt(),
                Friends = [.. p.ParseArray<Friend>()],
                Messages = [.. p.ParseArray<ConsoleMessage>()],
                CampaignMessages = [.. p.ParseArray<CampaignMessage>()],
                Requests = [.. p.ParseArray<FriendRequest>()],
            };
        }
    }

    public void Compose(in PacketWriter p)
    {
        if (p.Client is not ClientType.Shockwave)
        {
            p.WriteInt(UserLimit);
            p.WriteInt(NormalLimit);
            p.WriteInt(ExtendedLimit);
            p.WriteLength(Categories.Count);
            foreach (var (id, name) in Categories)
            {
                p.WriteInt(id);
                p.WriteString(name);
            }
        }
        else
        {
            p.WriteString(PersistentMessage);
            p.WriteInt(UserLimit);
            p.WriteInt(NormalLimit);
            p.WriteInt(ExtendedLimit);
            p.ComposeArray(Friends);
            p.ComposeArray(Messages);
            p.ComposeArray(CampaignMessages);
            p.ComposeArray(Requests);
        }
    }
}