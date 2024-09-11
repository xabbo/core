namespace Xabbo.Core.Events;

public sealed class AvatarUpdateEventArgs(IAvatar avatar, string previousFigure, Gender previousGender, string previousMotto)
    : AvatarEventArgs(avatar)
{
    public string PreviousFigure { get; } = previousFigure;
    public Gender PreviousGender { get; } = previousGender;
    public string PreviousMotto { get; } = previousMotto;
}
