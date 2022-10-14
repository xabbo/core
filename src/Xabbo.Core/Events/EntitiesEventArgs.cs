using System;
using System.Linq;
using System.Collections.Generic;

namespace Xabbo.Core.Events;

public class EntitiesEventArgs : EventArgs
{
    public IEntity[] Entities { get; }

    public EntitiesEventArgs(IEnumerable<IEntity> entities)
    {
        if (entities is IEntity[] array)
            Entities = array;
        else
            Entities = entities.ToArray();
    }
}
