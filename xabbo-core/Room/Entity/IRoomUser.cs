using System;

namespace Xabbo.Core
{
    public interface IRoomUser : IEntity
    {
        Gender Gender { get; }
        int GroupId { get; }
        string GroupName { get; }
        string FigureExtra { get; }
        int AchievementScore { get; }

        int RightsLevel { get; }
        bool HasRights { get; }
    }
}
