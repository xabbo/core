﻿namespace Xabbo.Core;

public interface IUser : IAvatar
{
    Gender Gender { get; }
    Id GroupId { get; }
    int GroupStatus { get; }
    string GroupName { get; }
    string FigureExtra { get; }
    int AchievementScore { get; }
    bool IsModerator { get; }

    int RightsLevel { get; }
    bool HasRights { get; }
}
