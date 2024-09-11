using System.Linq;
using System.Collections.Generic;

namespace Xabbo.Core.Events;

public sealed class AvatarsEventArgs(IEnumerable<IAvatar> avatars)
{
    public IAvatar[] Avatars { get; } = avatars.ToArray();
}
