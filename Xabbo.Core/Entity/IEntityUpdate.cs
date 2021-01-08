using System;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public interface IEntityUpdate : IPacketData
    {
        int Index { get; }
        Tile Location { get; }
        int HeadDirection { get; }
        int Direction { get; }
        string Status { get; }

        Stances Stance { get; }

        bool IsController { get; }
        int ControlLevel { get; }

        Tile? MovingTo { get; }

        bool SittingOnFloor { get; }

        double? ActionHeight { get; }

        Signs Sign { get; }
    }
}
