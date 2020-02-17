using System;

namespace Xabbo.Core.Events
{
    public class UserDataUpdatedEventArgs : RoomUserEventArgs
    {
        public string PreviousFigure { get; }
        public Gender PreviousGender { get; }
        public string PreviousMotto { get; }
        public int PreviousAchievementScore { get; }

        public bool FigureUpdated { get; }
        public bool GenderUpdated { get; }
        public bool MottoUpdated { get; }
        public bool AchievementScoreUpdated { get; }

        public UserDataUpdatedEventArgs(RoomUser user,
            string previousFigure, Gender previousGender,
            string previousMotto, int previousAchievementScore)
            : base(user)
        {
            PreviousFigure = previousFigure;
            PreviousGender = previousGender;
            PreviousMotto = previousMotto;
            PreviousAchievementScore = previousAchievementScore;

            FigureUpdated = !Equals(user.Figure, previousFigure);
            GenderUpdated = !Equals(user.Gender, previousGender);
            MottoUpdated = !Equals(user.Motto, previousMotto);
            AchievementScoreUpdated = !Equals(user.AchievementScore, previousAchievementScore);
        }
    }
}
