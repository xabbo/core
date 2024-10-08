﻿using System;

namespace Xabbo.Core;

public interface IPet : IAvatar
{
    int Breed { get; }
    Id OwnerId { get; }
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
