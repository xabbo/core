using System;

namespace Xabbo.Core.Events;

public class QueueEventArgs : EventArgs
{
    public int Position { get; }

    public QueueEventArgs(int position)
    {
        Position = position;
    }
}
