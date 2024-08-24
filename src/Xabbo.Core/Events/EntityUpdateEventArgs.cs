namespace Xabbo.Core.Events;

public sealed class EntityUpdateEventArgs(IEntity entity, string previousFigure, Gender previousGender, string previousMotto)
    : EntityEventArgs(entity)
{
    public string PreviousFigure { get; } = previousFigure;
    public Gender PreviousGender { get; } = previousGender;
    public string PreviousMotto { get; } = previousMotto;
}
