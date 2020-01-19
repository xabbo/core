using System;
using System.Linq;
using System.Collections.Generic;

namespace Xabbo.Core.Events
{
    public class EntitiesEventArgs : EventArgs
    {
        public Entity[] Entities { get; }

        public EntitiesEventArgs(IEnumerable<Entity> entities)
        {
            if (entities is Entity[] array)
                Entities = array;
            else
                Entities = entities.ToArray();
        }
    }
}
