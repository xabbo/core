namespace Xabbo.Core.Events;

public sealed class MutedEventArgs(MuteType type, int secondsLeft)
{
    public MuteType Type { get; } = type;
    public int SecondsLeft { get; } = secondsLeft;
}
