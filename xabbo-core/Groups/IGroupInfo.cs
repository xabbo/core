using System;

namespace Xabbo.Core
{
    public interface IGroupInfo
    {
        int Id { get; }
        string Name { get; }
        string Badge { get;}
        string ColorA { get; }
        string ColorB { get; }
        bool IsFavorite { get; }
        int OwnerId { get; }
    }
}
