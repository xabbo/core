using System;

using Xabbo.Messages;

namespace Xabbo.Core;

public interface IGroupInfo : IComposable
{
    long Id { get; }
    string Name { get; }
    string BadgeCode { get;}
    string PrimaryColor { get; }
    string SecondaryColor { get; }
    bool IsFavorite { get; }
    long OwnerId { get; }
    bool HasForum { get; }
}
