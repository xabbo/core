using System;

namespace Xabbo.Core
{
    public interface IRoomUser : IEntity
    {
        Gender Gender { get; }
        long GroupId { get; }
        int GroupStatus { get; }
        string GroupName { get; }
        string FigureExtra { get; }
        int AchievementScore { get; }
        bool IsModerator { get; }

        int RightsLevel { get; }
        bool HasRights { get; }
    }
}
