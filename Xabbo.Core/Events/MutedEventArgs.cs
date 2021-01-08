using System;

namespace Xabbo.Core.Events
{
    public class MutedEventArgs : EventArgs
    {
        public MuteType Type { get; }
        public int SecondsLeft { get; }

        public MutedEventArgs(MuteType type, int secondsLeft)
        {
            Type = type;
            SecondsLeft = secondsLeft;
        }
    }
}
