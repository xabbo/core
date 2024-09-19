using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Received when activity points are loaded.
/// <para/>
/// Supported on:
/// <list type="bullet">
/// <item>Flash</item>
/// </list>
/// </summary>
public sealed record ActivityPointsMsg(ActivityPoints ActivityPoints) : IMessage<ActivityPointsMsg>
{
    static bool IMessage<ActivityPointsMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<ActivityPointsMsg>.Identifier => In.ActivityPoints;

    static ActivityPointsMsg IParser<ActivityPointsMsg>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfOrigins(p.Client);
        return new ActivityPointsMsg(p.Parse<ActivityPoints>());
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfOrigins(p.Client);
        p.Compose(ActivityPoints);
    }
}
