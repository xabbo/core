namespace Xabbo.Core;

/// <summary>
/// Defines basic information about a group.
/// </summary>
public interface IGroupInfo
{
    Id Id { get; }
    string Name { get; }
    string BadgeCode { get; }
    string PrimaryColor { get; }
    string SecondaryColor { get; }
    bool IsFavorite { get; }
    Id OwnerId { get; }
    bool HasForum { get; }
}
