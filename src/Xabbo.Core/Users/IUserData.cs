namespace Xabbo.Core;

/// <summary>
/// Represents information about the current user.
/// </summary>
public interface IUserData
{
    Id Id { get; }
    string Name { get; }
    string Figure { get; }
    Gender Gender { get; }
    string Motto { get; }
    string RealName { get; set; }
    bool DirectMail { get; set; }
    int TotalRespects { get; }
    int RespectsLeft { get; }
    int ScratchesLeft { get; }
    bool StreamPublishingAllowed { get; }
    string LastAccessDate { get; }
    bool IsNameChangeable { get; }
    bool IsSafetyLocked { get; }
}
