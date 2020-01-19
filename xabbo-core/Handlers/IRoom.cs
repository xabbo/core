using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabbo.Core.Events;

namespace Xabbo.Core
{
    public interface IRoom
    {
        int Id { get; }
        bool IsDataLoaded { get; }
        RoomData Data { get; }
        string Name { get; }
        int OwnerId { get; }
        string OwnerName { get; }

        Tile DoorTile { get; }
        HeightMap RelativeMap { get; }
        FloorPlan HeightMap { get; }
        IEnumerable<FloorItem> FloorItems { get; }
        IEnumerable<WallItem> WallItems { get; }

        IEnumerable<Entity> Entities { get; }
        IEnumerable<RoomUser> Users { get; }
        IEnumerable<Pet> Pets { get; }
        IEnumerable<PublicBot> PublicBots { get; }
        IEnumerable<UserBot> UserBots { get; }

        /*#region - Events -
        // Items

        // Entities
        event EventHandler<EntitiesEventArgs> EntitiesAdded;
        event EventHandler<EntityEventArgs> EntityRemoved;
        event EventHandler<EntitiesEventArgs> EntitiesUpdated;
        event EventHandler<EntityChatEventArgs> EntityChat;
        // TODO: Create appropriate EventArgs
        event EventHandler<EntityEventArgs> EntityDance;
        event EventHandler<EntityEventArgs> EntitySign;
        #endregion*/


    }
}
