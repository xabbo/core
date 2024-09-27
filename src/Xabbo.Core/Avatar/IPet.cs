namespace Xabbo.Core;

/// <summary>
/// Represents a pet in a room.
/// </summary>
public interface IPet : IAvatar
{
    int Breed { get; }
    /// <summary>
    /// The ID of the owner of the pet.
    /// </summary>
    Id OwnerId { get; }
    /// <summary>
    /// The name of the owner of the pet.
    /// </summary>
    string OwnerName { get; }
    int RarityLevel { get; }
    bool HasSaddle { get; }
    bool IsRiding { get; }
    bool CanBreed { get; }
    bool CanHarvest { get; }
    bool CanRevive { get; }
    bool HasBreedingPermission { get; }
    int Level { get; }
    string Posture { get; }
}
