using System;
using System.Collections.Generic;
using Xabbo.Messages;

namespace Xabbo.Core
{
    public interface IUserProfile : IComposable
    {
        long Id { get; }
        string Name { get; }
        string Figure { get; }
        string Motto { get; }
        string Created { get; }
        int ActivityPoints { get; }
        int Friends { get; }
        bool IsFriend { get; }
        bool IsFriendRequestSent { get; }
        bool IsOnline { get; }
        IReadOnlyList<IGroupInfo> Groups { get; }
        TimeSpan LastLogin { get; }
    }
}
