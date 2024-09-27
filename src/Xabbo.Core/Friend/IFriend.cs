namespace Xabbo.Core;

/// <summary>
/// Represents a friend in the user's friend list.
/// </summary>
public interface IFriend
{
    /// <summary>
    /// The ID of the friend.
    /// </summary>
    Id Id { get; }

    /// <summary>
    /// The name of the friend.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The gender of the friend.
    /// </summary>
    Gender Gender { get; }

    /// <summary>
    /// Whether the friend is online.
    /// </summary>
    bool IsOnline { get; }

    /// <summary>
    /// Whether the friend can currently be followed.
    /// </summary>
    bool CanFollow { get; }

    /// <summary>
    /// The figure string of the friend.
    /// </summary>
    string Figure { get; }

    /// <summary>
    /// The category in the friend list that the friend belongs to.
    /// </summary>
    int CategoryId { get; }

    /// <summary>
    /// The motto of the friend.
    /// </summary>
    string Motto { get; }

    /// <summary>
    /// Whether the friend is accepting offline messages.
    /// </summary>
    bool IsAcceptingOfflineMessages { get; }

    /// <summary>
    /// Whether the friend is a club member.
    /// </summary>
    bool IsVipMember { get; }

    bool IsPocketHabboUser { get; }

    /// <summary>
    /// Gets the relation of the friend.
    /// </summary>
    Relation Relation { get; }
}
