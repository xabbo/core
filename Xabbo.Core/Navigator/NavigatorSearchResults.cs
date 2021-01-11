using System;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class NavigatorSearchResults : List<NavigatorSearchResultList>
    {
        public static NavigatorSearchResults Parse(IReadOnlyPacket packet) => new NavigatorSearchResults(packet);

        public string Category { get; set; }
        public string Filter { get; set; }

        public NavigatorSearchResults(string category, string filter)
        {
            Category = category;
            Filter = filter;
        }

        protected NavigatorSearchResults(IReadOnlyPacket packet)
        {
            Category = packet.ReadString();
            Filter = packet.ReadString();
            short n = packet.ReadShort();
            for (int i = 0; i < n; i++)
                Add(NavigatorSearchResultList.Parse(packet));
        }

        public IEnumerable<RoomInfo> GetRooms()
        {
            var hashSet = new HashSet<long>();
            foreach (var roomInfo in this.SelectMany(list => list))
            {
                if (hashSet.Add(roomInfo.Id))
                    yield return roomInfo;
            }
        }

        public IEnumerable<RoomInfo> FindRooms(
            string name = null,
            string description = null,
            int? ownerId = null,
            string owner = null,
            RoomAccess? access = null,
            TradePermissions? trading = null,
            RoomCategory? category = null,
            int? groupId = null,
            string group = null)
        {
            foreach (var roomInfo in GetRooms())
            {
                if (name != null && !roomInfo.Name.ToLower().Contains(name.ToLower())) continue;
                if (description != null && !roomInfo.Description.ToLower().Contains(description.ToLower())) continue;
                if (ownerId.HasValue && roomInfo.OwnerId != ownerId) continue;
                if (owner != null && !roomInfo.OwnerName.Equals(owner, StringComparison.InvariantCultureIgnoreCase)) continue;
                if (access.HasValue && roomInfo.Access != access) continue;
                if (trading.HasValue && roomInfo.Trading != trading) continue;
                if (category.HasValue && roomInfo.Category != category) continue;
                if (groupId.HasValue && (!roomInfo.IsGroupHomeRoom || roomInfo.GroupId != groupId)) continue;
                if (group != null && (!roomInfo.IsGroupHomeRoom || !roomInfo.GroupName.ToLower().Contains(group.ToLower()))) continue;
                yield return roomInfo;
            }
        }

        public RoomInfo FindRoom(string name) => GetRooms().FirstOrDefault(roomInfo => roomInfo.Name.ToLower().Contains(name.ToLower()));
    }
}
