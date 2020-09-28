using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xabbo.Core
{
    public interface IGroupData
    {
        int Id { get; }
        bool CanLeave { get; }
        GroupType Type { get; }
        string Name { get; }
        string Description { get; }
        string Badge { get; }
        int HomeRoomId { get; }
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
        bool CanViewForum { get; }
    }
}
