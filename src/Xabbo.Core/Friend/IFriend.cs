using System.ComponentModel;

namespace Xabbo.Core;

/// <summary>
/// Contains information about the user's friend.
/// </summary>
public interface IFriend
{
    /// <summary>
    /// Gets the ID of the friend.
    /// </summary>
    Id Id { get; }
    /// <summary>
    /// Gets the name of the friend.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Gets the gender of the friend.
    /// </summary>
    Gender Gender { get; }
    /// <summary>
    /// Gets whether the friend is online or not.
    /// </summary>
    bool IsOnline { get; }
    /// <summary>
    /// Gets whether the friend can be followed or not.
    /// </summary>
    bool CanFollow { get; }
    /// <summary>
    /// Gets the figure string of the friend.
    /// </summary>
    string Figure { get; }
    /// <summary>
    /// Gets the category in the friend list that the friend belongs to.
    /// </summary>
    int CategoryId { get; }
    /// <summary>
    /// Gets the motto of the friend.
    /// </summary>
    string Motto { get; }
    string RealName { get; }
    string FacebookId { get; }
    /// <summary>
    /// Gets whether the friend is accepting offline messages or not.
    /// </summary>
    bool IsAcceptingOfflineMessages { get; }
    bool IsVipMember { get; }
    bool IsPocketHabboUser { get; }
    /// <summary>
    /// Gets the relation of the friend.
    /// </summary>
    Relation Relation { get; }
}
