using System;

namespace Xabbo.Core.Events
{
    public class EntityExpressionEventArgs : EntityEventArgs
    {
        public Expressions Expression { get; }

        public EntityExpressionEventArgs(IEntity entity, Expressions expression)
            :  base(entity)
        {
            Expression = expression;
        }
    }
}
