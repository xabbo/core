using System;

namespace Xabbo.Core;

/// <summary>
/// Represents a member of a group.
/// </summary>
public interface IGroupMember
{
    GroupMemberType Type { get; }
    Id Id { get; }
    string Name { get; }
    string Figure { get; }
    DateTime Joined { get; }
}
