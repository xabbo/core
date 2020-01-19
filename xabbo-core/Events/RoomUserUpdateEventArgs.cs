using System;

namespace Xabbo.Core.Events
{
    public class RoomUserUpdateEventArgs : RoomUserEventArgs
    {
        public string PreviousFigure { get; }
        public Gender PreviousGender { get; }
        public string PreviousMotto { get; }
        
        public RoomUserUpdateEventArgs(RoomUser user, string previousFigure, Gender previousGender, string previousMotto)
            : base(user)
        {
            PreviousFigure = previousFigure;
            PreviousGender = previousGender;
            PreviousMotto = previousMotto;
        }
    }
}
