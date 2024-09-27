namespace Xabbo.Core;

/// <summary>
/// Defines extended information about a group.
/// </summary>
public interface IGroupData
{
    Id Id { get; }
    bool IsGuild { get; }
    GroupType Type { get; }
    string Name { get; }
    string Description { get; }
    string Badge { get; }
    Id HomeRoomId { get; }
    string HomeRoomName { get; }
    GroupMemberStatus MemberStatus { get; }
    int MemberCount { get; }
    bool IsFavourite { get; }
    string Created { get; }
    bool IsOwner { get; }
    bool IsAdmin { get; }
    string OwnerName { get; }
    bool CanDecorateHomeRoom { get; }
    int PendingRequests { get; }
    bool HasForum { get; }
}
