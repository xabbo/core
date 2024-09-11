using System;

namespace Xabbo.Core.Events;

public sealed class AvatarDataUpdatedEventArgs : AvatarEventArgs
{
    public string PreviousFigure { get; }
    public Gender PreviousGender { get; }
    public string PreviousMotto { get; }
    public int PreviousAchievementScore { get; }

    public bool FigureUpdated { get; }
    public bool GenderUpdated { get; }
    public bool MottoUpdated { get; }
    public bool AchievementScoreUpdated { get; }

    public AvatarDataUpdatedEventArgs(IAvatar avatar,
        string previousFigure, Gender previousGender,
        string previousMotto, int previousAchievementScore)
        : base(avatar)
    {
        PreviousFigure = previousFigure;
        PreviousGender = previousGender;
        PreviousMotto = previousMotto;
        PreviousAchievementScore = previousAchievementScore;

        Gender currentGender = previousGender;
        int currentAchievementScore = previousAchievementScore;
        if (avatar is IBot bot)
        {
            currentGender = bot.Gender;
        }
        else if (avatar is IUser user)
        {
            currentGender = user.Gender;
            currentAchievementScore = user.AchievementScore;
        }

        FigureUpdated = !Equals(avatar.Figure, previousFigure);
        GenderUpdated = !Equals(currentGender, previousGender);
        MottoUpdated = !Equals(avatar.Motto, previousMotto);
        AchievementScoreUpdated = !Equals(currentAchievementScore, previousAchievementScore);
    }
}
