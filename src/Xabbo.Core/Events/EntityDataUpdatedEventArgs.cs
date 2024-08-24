using System;

namespace Xabbo.Core.Events;

public sealed class EntityDataUpdatedEventArgs : EntityEventArgs
{
    public string PreviousFigure { get; }
    public Gender PreviousGender { get; }
    public string PreviousMotto { get; }
    public int PreviousAchievementScore { get; }

    public bool FigureUpdated { get; }
    public bool GenderUpdated { get; }
    public bool MottoUpdated { get; }
    public bool AchievementScoreUpdated { get; }

    public EntityDataUpdatedEventArgs(IEntity entity,
        string previousFigure, Gender previousGender,
        string previousMotto, int previousAchievementScore)
        : base(entity)
    {
        PreviousFigure = previousFigure;
        PreviousGender = previousGender;
        PreviousMotto = previousMotto;
        PreviousAchievementScore = previousAchievementScore;

        Gender currentGender = previousGender;
        int currentAchievementScore = previousAchievementScore;
        if (entity is IBot bot)
        {
            currentGender = bot.Gender;
        }
        else if (entity is IRoomUser user)
        {
            currentGender = user.Gender;
            currentAchievementScore = user.AchievementScore;
        }

        FigureUpdated = !Equals(entity.Figure, previousFigure);
        GenderUpdated = !Equals(currentGender, previousGender);
        MottoUpdated = !Equals(entity.Motto, previousMotto);
        AchievementScoreUpdated = !Equals(currentAchievementScore, previousAchievementScore);
    }
}
