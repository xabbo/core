using System.Linq;
using System.Collections.Generic;

namespace Xabbo.Core.Events;

public sealed class EntitiesEventArgs(IEnumerable<IEntity> entities)
{
    public IEntity[] Entities { get; } = entities.ToArray();
}
