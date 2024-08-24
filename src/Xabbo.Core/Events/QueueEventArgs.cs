namespace Xabbo.Core.Events;

public sealed class QueueEventArgs(int position)
{
    public int Position { get; } = position;
}
