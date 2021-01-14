using System;

namespace Xabbo.Core
{
    public interface IGroupMember
    {
        GroupMemberType Type { get; }
        long Id { get; }
        string Name { get; }
        string Figure { get; }
        DateTime Joined { get; }
    }
}
