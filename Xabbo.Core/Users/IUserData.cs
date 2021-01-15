using System;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    /// <summary>
    /// The user's own data that is sent upon requesting user data.
    /// </summary>
    public interface IUserData : IPacketData
    {
        long Id { get; }
        string Name { get; }
        string Figure { get; }
        Gender Gender { get; }
        string Motto { get; }
        int TotalRespects { get; }
        int RespectsLeft { get; }
        int ScratchesLeft { get; }
        string LastLogin { get; } // @Cleanup LastAccess ?
        bool IsNameChangeable { get; }
        bool IsSafetyLocked { get; }
    }
}
