using System;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public interface IFurni : IItem, IWritable
    {
        int OwnerId { get; }
        string OwnerName { get; }

        int State { get; }

        int SecondsToExpiration { get; }
        FurniUsage Usage { get; }
    }
}
