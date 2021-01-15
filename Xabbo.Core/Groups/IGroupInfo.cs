using System;
using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    public interface IGroupInfo : IPacketData
    {
        int Id { get; }
        string Name { get; }
        string BadgeCode { get;}
        string PrimaryColor { get; }
        string SecondaryColor { get; }
        bool IsFavorite { get; }
        int OwnerId { get; }
        bool HasForum { get; }
    }
}
