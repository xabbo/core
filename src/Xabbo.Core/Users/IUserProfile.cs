using System;
using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Represents the in-game profile of user.
/// </summary>
public interface IUserProfile
{
    Id Id { get; }
    string Name { get; }
    string Figure { get; }
    string Motto { get; }
    string Created { get; }
    int ActivityPoints { get; }
    int Friends { get; }
    bool IsFriend { get; }
    bool IsFriendRequestSent { get; }
    bool IsOnline { get; }
    IReadOnlyList<IGroupInfo> Groups { get; }
    TimeSpan LastLogin { get; }
    int Level { get; }
    int StarGems { get; }
}
