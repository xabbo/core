using System;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// The current user's information that is sent upon requesting user data.
/// </summary>
public interface IUserData : IComposable
{
    long Id { get; }
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
