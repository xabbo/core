using System;

namespace Xabbo.Core.Events;

public class EntityUpdateEventArgs : EntityEventArgs
{
    public string PreviousFigure { get; }
    public Gender PreviousGender { get; }
    public string PreviousMotto { get; }
    
    public EntityUpdateEventArgs(IEntity entity, string previousFigure, Gender previousGender, string previousMotto)
        : base(entity)
    {
        PreviousFigure = previousFigure;
        PreviousGender = previousGender;
        PreviousMotto = previousMotto;
    }
}
