using System;

namespace Xabbo.Core
{
    public interface IGroupMember
    {
        GroupMemberType Type { get; }
        int Id { get; }
        string Name { get; }
        string Figure { get; }
        DateTime Joined { get; }
    }
}
