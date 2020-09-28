using System;

namespace Xabbo.Core
{
    public interface IEntityUpdate
    {
        int Index { get; }
        ITile Location { get; }
        int HeadDirection { get; }
        int Direction { get; }
        string Status { get; }

        Stances Stance { get; }

        bool IsController { get; }
        int ControlLevel { get; }

        ITile MovingTo { get; }

        bool SittingOnFloor { get; }

        double ActionHeight { get; }

        Signs Sign { get; }
    }
}
