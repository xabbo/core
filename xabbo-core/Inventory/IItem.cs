using System;

namespace Xabbo.Core
{
    public interface IItem
    {
        FurniType Type { get; }
        int Kind { get; }
        int Id { get; }
    }
}
