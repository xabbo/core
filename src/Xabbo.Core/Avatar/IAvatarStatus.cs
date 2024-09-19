namespace Xabbo.Core;

public interface IAvatarStatus
{
    int Index { get; }
    Tile Location { get; }
    int HeadDirection { get; }
    int Direction { get; }

    Stances Stance { get; }

    bool IsController { get; }
    int ControlLevel { get; }

    bool IsTrading { get; }

    Tile? MovingTo { get; }

    bool SittingOnFloor { get; }

    double? ActionHeight { get; }

    Signs Sign { get; }
}
