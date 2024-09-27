namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.AvatarChanged"/> event.
/// </summary>
public sealed class AvatarChangedEventArgs : AvatarEventArgs
{
    /// <summary>
    /// Gets the avatar's previous figure.
    /// </summary>
    public string PreviousFigure { get; }

    /// <summary>
    /// Gets the avatar's previous gender.
    /// </summary>
    public Gender PreviousGender { get; }

    /// <summary>
    /// Gets the avatar's previous motto.
    /// </summary>
    public string PreviousMotto { get; }

    /// <summary>
    /// Gets the avatar's previous achievement score.
    /// </summary>
    public int PreviousAchievementScore { get; }

    /// <summary>
    /// Gets whether the avatar's figure was updated.
    /// </summary>
    public bool FigureUpdated { get; }

    /// <summary>
    /// Gets whether the avatar's gender was updated.
    /// </summary>
    public bool GenderUpdated { get; }

    /// <summary>
    /// Gets whether the avatar's motto was updated.
    /// </summary>
    public bool MottoUpdated { get; }

    /// <summary>
    /// Gets whether the avatar's achievement score was updated.
    /// </summary>
    public bool AchievementScoreUpdated { get; }

    public AvatarChangedEventArgs(IAvatar avatar,
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

        FigureUpdated = avatar.Figure != previousFigure;
        GenderUpdated = currentGender != previousGender;
        MottoUpdated = avatar.Motto != previousMotto;
        AchievementScoreUpdated = currentAchievementScore != previousAchievementScore;
    }
}
