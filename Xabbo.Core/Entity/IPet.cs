using System;

namespace Xabbo.Core
{
    public interface IPet : IEntity
    {
        int Breed { get; }
        long OwnerId { get; }
        string OwnerName { get; }
        int RarityLevel { get; }
        int Level { get; }
        string Stance { get; }
    }
}
