using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Core.Events;
using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    /// <summary>
    /// Manages and tracks entities in the room.
    /// </summary>
    [Dependencies(typeof(RoomManager))]
    public class EntityManager : XabboComponent
    {
        public enum Features
        {
            Tracking,
            StateTracking
        }

        private RoomManager roomManager;

        private readonly ConcurrentDictionary<int, Entity> entities = new ConcurrentDictionary<int, Entity>();

        /// <summary>
        /// Gets the entities in the room.
        /// </summary>
        public IEnumerable<IEntity> Entities => entities.Select(x => x.Value);
        /// <summary>
        /// Gets the users in the room.
        /// </summary>
        public IEnumerable<IRoomUser> Users => Entities.OfType<RoomUser>();
        /// <summary>
        /// Gets the pets in the room.
        /// </summary>
        public IEnumerable<IPet> Pets => Entities.OfType<Pet>();
        /// <summary>
        /// Gets the bots in the room.
        /// </summary>
        public IEnumerable<IBot> Bots => Entities.OfType<Bot>();

        /// <summary>
        /// Gets the entity of the specified type with the specified index, or <c>null</c> if it does not exist.
        /// </summary>
        public T GetEntityByIndex<T>(int index) where T : class, IEntity => GetEntityByIndex(index) as T;
        /// <summary>
        /// Gets the entity of the specified type with the specified ID, or <c>null</c> if it does not exist.
        /// </summary>
        public T GetEntity<T>(int id) where T : IEntity
            => Entities.OfType<T>().FirstOrDefault(e => e.Id == id);
        /// <summary>
        /// Gets the entity of the specified type with the specified name, or <c>null</c> if it does not exist.
        /// </summary>
        public T GetEntity<T>(string name) where T : IEntity
            => Entities.OfType<T>().FirstOrDefault(e => e.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        /// <summary>
        /// Gets the entity with the specified index, or <c>null</c> if it does not exist.
        /// </summary>
        public IEntity GetEntityByIndex(int index) => entities.TryGetValue(index, out Entity e) ? e : null;
        /// <summary>
        /// Gets the entity with the specified ID, or <c>null</c> if it does not exist.
        /// </summary>
        public IEntity GetEntity(int id) => Entities.FirstOrDefault(e => e.Id == id);
        /// <summary>
        /// Gets the entity with the specified name, or <c>null</c> if it does not exist.
        /// </summary>
        public IEntity GetEntity(string name) => Entities.FirstOrDefault(e => e.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        /// <summary>
        /// Attempts to get the entity with the specified index.
        /// </summary>
        public bool TryGetEntityByIndex(int index, out IEntity entity)
        {
            if (entities.TryGetValue(index, out Entity result))
            {
                entity = result;
                return true;
            }
            else
            {
                entity = default;
                return false;
            }
        }

        /// <summary>
        /// Attempts to get the entity of the specified type with the specified index.
        /// </summary>
        public bool TryGetEntityByIndex<T>(int index, out T entity) where T : IEntity
        {
            if (entities.TryGetValue(index, out Entity e))
            {
                entity = (T)(IEntity)e;
                return true;
            }
            else
            {
                entity = default;
                return false;
            }
        }

        /// <summary>
        /// Attempts to get the entity of the specified type with the specified ID.
        /// </summary>
        public bool TryGetEntity<T>(int id, out T entity) where T : IEntity
            => (entity = GetEntity<T>(id)) != null;

        /// <summary>
        /// Attempts to get the entity of the specified type with the specified name.
        /// </summary>
        public bool TryGetEntity<T>(string name, out T entity) where T : IEntity
            => (entity = GetEntity<T>(name)) != null;

        #region - Events -
        /// <summary>
        /// Invoked when an entity has been added to the room.
        /// </summary>
        public event EventHandler<EntityEventArgs> EntityAdded;
        /// <summary>
        /// Invoked when entities have been added to the room.
        /// </summary>
        public event EventHandler<EntitiesEventArgs> EntitiesAdded;
        /// <summary>
        /// Invoked when an entity in the room is updated.
        /// </summary>
        public event EventHandler<EntityEventArgs> EntityUpdated;
        /// <summary>
        /// Invoked when entities in the room are updated.
        /// </summary>
        public event EventHandler<EntitiesEventArgs> EntitiesUpdated;
        /// <summary>
        /// Invoked when an entity slides along a roller.
        /// </summary>
        public event EventHandler<EntitySlideEventArgs> EntitySlide;
        /// <summary>
        /// Invoked when a user's figure, motto or achievement score is updated.
        /// </summary>
        public event EventHandler<UserDataUpdatedEventArgs> UserDataUpdated;
        /// <summary>
        /// Invoked when an entity's name changes.
        /// </summary>
        public event EventHandler<EntityNameChangedEventArgs> EntityNameChanged;
        /// <summary>
        /// Invoked when an entity's idle status updates.
        /// </summary>
        public event EventHandler<EntityIdleEventArgs> EntityIdle;
        /// <summary>
        /// Invoked when an entity's dance updates.
        /// </summary>
        public event EventHandler<EntityDanceEventArgs> EntityDance;
        /// <summary>
        /// Invoked when an entity's hand item updates.
        /// </summary>
        public event EventHandler<EntityHandItemEventArgs> EntityHandItem;
        /// <summary>
        /// Invoked when an entity's effect updates.
        /// </summary>
        public event EventHandler<EntityEffectEventArgs> EntityEffect;
        /// <summary>
        /// Invoked when an entity performs an action.
        /// </summary>
        public event EventHandler<EntityActionEventArgs> EntityAction;
        /// <summary>
        /// Invoked when an entity's typing status updates.
        /// </summary>
        public event EventHandler<EntityTypingEventArgs> EntityTyping;
        /// <summary>
        /// Invoked when an entity is removed from the room.
        /// </summary>
        public event EventHandler<EntityEventArgs> EntityRemoved;

        protected virtual void OnEntityAdded(IEntity entity)
            => EntityAdded?.Invoke(this, new EntityEventArgs(entity));
        protected virtual void OnEntitiesAdded(IEnumerable<IEntity> entities)
            => EntitiesAdded?.Invoke(this, new EntitiesEventArgs(entities));
        protected virtual void OnEntityUpdated(IEntity entity)
            => EntityUpdated?.Invoke(this, new EntityEventArgs(entity));
        protected virtual void OnEntitiesUpdated(IEnumerable<IEntity> entities)
            => EntitiesUpdated?.Invoke(this, new EntitiesEventArgs(entities));
        protected virtual void OnEntitySlide(IEntity entity, Tile previousTile)
            => EntitySlide?.Invoke(this, new EntitySlideEventArgs(entity, previousTile));
        protected virtual void OnUserDataUpdated(IRoomUser user,
            string previousFigure, Gender previousGender,
            string previousMotto, int previousAchievementScore)
            => UserDataUpdated?.Invoke(this, new UserDataUpdatedEventArgs(
                user, previousFigure, previousGender,
                previousMotto, previousAchievementScore
            ));
        protected virtual void OnEntityNameChanged(IEntity entity, string previousName)
            => EntityNameChanged?.Invoke(this, new EntityNameChangedEventArgs(entity, previousName));
        protected virtual void OnEntityIdle(IEntity entity, bool wasIdle)
            => EntityIdle?.Invoke(this, new EntityIdleEventArgs(entity, wasIdle));
        protected virtual void OnEntityDance(IEntity entity, int previousDance)
            => EntityDance?.Invoke(this, new EntityDanceEventArgs(entity, previousDance));
        protected virtual void OnEntityHandItem(IEntity entity, int previousItem)
            => EntityHandItem?.Invoke(this, new EntityHandItemEventArgs(entity, previousItem));
        protected virtual void OnEntityEffect(IEntity entity, int previousEffect)
            => EntityEffect?.Invoke(this, new EntityEffectEventArgs(entity, previousEffect));
        protected virtual void OnEntityAction(IEntity entity, Actions action)
            => EntityAction?.Invoke(this, new EntityActionEventArgs(entity, action));
        protected virtual void OnEntityTyping(IEntity entity, bool wasTyping)
            => EntityTyping?.Invoke(this, new EntityTypingEventArgs(entity, wasTyping));
        protected virtual void OnEntityRemoved(IEntity entity)
            => EntityRemoved?.Invoke(this, new EntityEventArgs(entity));
        #endregion

        public EntityManager() { }

        protected override void OnInitialize()
        {
            roomManager = GetComponent<RoomManager>();
            roomManager.Left += Room_Left;
        }

        private void Room_Left(object sender, EventArgs e)
        {
            DebugUtil.Log("clearing entities");

            entities.Clear();
        }

        public void Show(IEntity entity)
        {
            var e = entity as Entity;
            if (e == null) return;

            if (e.IsHidden)
            {
                e.IsHidden = false;
                SendLocalAsync(In.RoomUsers, 1, e);
            }
        }

        public void Show(IEnumerable<IEntity> entities)
        {
            var shown = new List<IEntity>();
            foreach (var entity in entities.OfType<Entity>())
            {
                if (entity.IsHidden)
                {
                    entity.IsHidden = false;
                    shown.Add(entity);
                }
            }

            SendLocalAsync(In.RoomUsers, shown);
        }

        public void Hide(IEntity entity)
        {
            var e = entity as Entity;
            if (e == null) return;

            if (!e.IsHidden)
            {
                e.IsHidden = true;
                // @Update SendLocalAsync(In.RoomUserRemove, e.Index.ToString());
            }
        }

        [InterceptIn(nameof(Incoming.RoomUsers))]
        private void HandleRoomUsers(InterceptArgs e)
        {
            if (!roomManager.IsLoadingRoom && !roomManager.IsInRoom)
                return;

            var newEntities = new List<Entity>();

            int n = e.Packet.ReadInt();
            for (int i = 0; i < n; i++)
            {
                var entity = Entity.Parse(e.Packet);
                if (entities.TryAdd(entity.Index, entity))
                {
                    newEntities.Add(entity);
                    OnEntityAdded(entity);
                }
                else
                {
                    DebugUtil.Log($"failed to add entity {entity.Index} {entity.Name} (id:{entity.Id})");
                }
            }

            if (newEntities.Count > 0)
            {
                OnEntitiesAdded(newEntities);
            }
        }

        // @Update [InterceptIn(nameof(Incoming.RoomUserRemove))]
        private void HandleEntityRemove(InterceptArgs e)
        {
            if (!roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            int index = int.Parse(e.Packet.ReadString());
            if (entities.TryRemove(index, out Entity entity))
            {
                OnEntityRemoved(entity);
            }
            else
            {
                DebugUtil.Log($"failed to remove entity {index}");
            }
        }

        // @Update [Group(Features.Tracking), InterceptIn(nameof(Incoming.RoomUserStatus))]
        private void HandleEntityUpdate(InterceptArgs e)
        {
            if (!roomManager.IsInRoom) return;

            var updatedEntities = new List<IEntity>();

            int n = e.Packet.ReadInt();
            for (int i = 0; i < n; i++)
            {
                var entityUpdate = EntityStatusUpdate.Parse(e.Packet);
                if (!entities.TryGetValue(entityUpdate.Index, out Entity entity))
                {
                    DebugUtil.Log($"failed to find entity {entityUpdate.Index} to update");
                    continue;
                }

                entity.Update(entityUpdate);
                updatedEntities.Add(entity);

                OnEntityUpdated(entity);
            }

            OnEntitiesUpdated(updatedEntities);
        }

        // @Update [Group(Features.Tracking), InterceptIn(nameof(Incoming.ObjectOnRoller))]
        private void HandleObjectOnRoller(InterceptArgs e)
        {
            if (!roomManager.IsInRoom) return;

            var rollerUpdate = RollerUpdate.Parse(e.Packet);

            if (rollerUpdate.Type == RollerUpdateType.MovingEntity ||
                rollerUpdate.Type == RollerUpdateType.StationaryEntity)
            {
                if (entities.TryGetValue(rollerUpdate.EntityIndex, out Entity entity))
                {
                    var previousTile = entity.Location;
                    entity.Location = new Tile(rollerUpdate.TargetX, rollerUpdate.TargetY, rollerUpdate.EntityTargetZ);

                    OnEntitySlide(entity, previousTile);
                }
                else
                {
                    DebugUtil.Log($"failed to find entity {rollerUpdate.EntityIndex} to update");
                }
            }
        }

        // @Update [Group(Features.StateTracking), InterceptIn(nameof(Incoming.RoomUserData))]
        private void HandleUserDataUpdated(InterceptArgs e)
        {
            if (!roomManager.IsInRoom) return;

            int index = e.Packet.ReadInt();
            if (TryGetEntityByIndex(index, out RoomUser user))
            {
                string previousFigure = user.Figure;
                Gender previousGender = user.Gender;
                string previousMotto = user.Motto;
                int previousAchievementScore = user.AchievementScore;

                user.Figure = e.Packet.ReadString();
                user.Gender = H.ToGender(e.Packet.ReadString());
                user.Motto = e.Packet.ReadString();
                user.AchievementScore = e.Packet.ReadInt();

                OnUserDataUpdated(user,
                    previousFigure, previousGender,
                    previousMotto, previousAchievementScore
                );
            }
            else
            {
                DebugUtil.Log($"failed to find entity {index} to update");
            }
        }

        // @Update [Group(Features.StateTracking), InterceptIn(nameof(Incoming.RoomUserNameChanged))]
        private void HandleRoomUserNameChanged(InterceptArgs e)
        {
            if (!roomManager.IsInRoom) return;

            int id = e.Packet.ReadInt();
            int index = e.Packet.ReadInt();
            string newName = e.Packet.ReadString();

            if (TryGetEntityByIndex(index, out Entity entity))
            {
                string previousName = entity.Name;
                entity.Name = newName;
                OnEntityNameChanged(entity, previousName);
            }
            else
            {
                DebugUtil.Log($"failed to find entity {index} to update");
            }
        }

        // @Update [Group(Features.StateTracking), InterceptIn(nameof(Incoming.RoomUnitIdle))]
        private void HandleEntityIdle(InterceptArgs e)
        {
            if (!roomManager.IsInRoom) return;

            int index = e.Packet.ReadInt();
            if (TryGetEntityByIndex(index, out Entity entity))
            {
                bool wasIdle = entity.IsIdle;
                entity.IsIdle = e.Packet.ReadBool();
                OnEntityIdle(entity, wasIdle);
            }
            else
            {
                DebugUtil.Log($"failed to find entity {index} to update");
            }
        }

        // @Update [Group(Features.StateTracking), InterceptIn(nameof(Incoming.RoomUserDance))]
        private void HandleEntityDance(InterceptArgs e)
        {
            if (!roomManager.IsInRoom) return;

            int index = e.Packet.ReadInt();
            var entity = GetEntityByIndex(index) as Entity;
            if (entity == null)
            {
                DebugUtil.Log($"failed to find entity {index} to update");
                return;
            }

            int previousDance = entity.Dance;
            entity.Dance = e.Packet.ReadInt();

            OnEntityDance(entity, previousDance);
        }

        // @Update [Group(Features.StateTracking), InterceptIn(nameof(Incoming.RoomUserAction))]
        private void HandleEntityAction(InterceptArgs e)
        {
            if (!roomManager.IsInRoom) return;

            int index = e.Packet.ReadInt();
            if (TryGetEntityByIndex(index, out Entity entity))
            {
                OnEntityAction(entity, (Actions)e.Packet.ReadInt());
            }
            else
            {
                DebugUtil.Log($"failed to find entity {index} to update");
            }
        }

        // @Update [Group(Features.StateTracking), InterceptIn(nameof(Incoming.RoomUserHandItem))]
        private void HandleEntityHandItem(InterceptArgs e)
        {
            if (!roomManager.IsInRoom) return;

            int index = e.Packet.ReadInt();
            if (TryGetEntityByIndex(index, out Entity entity))
            {
                int previousItem = entity.HandItem;
                entity.HandItem = e.Packet.ReadInt();
                OnEntityHandItem(entity, previousItem);
            }
            else
            {
                DebugUtil.Log($"failed to find entity {index} to update");
            }
        }

        // @Update [Group(Features.StateTracking), InterceptIn(nameof(Incoming.RoomUserEffect))]
        private void HandleEntityEffect(InterceptArgs e)
        {
            if (!roomManager.IsInRoom) return;

            int index = e.Packet.ReadInt();
            if (TryGetEntityByIndex(index, out Entity entity))
            {
                int previousEffect = entity.Effect;
                entity.Effect = e.Packet.ReadInt();
                OnEntityEffect(entity, previousEffect);
            }
            else
            {
                DebugUtil.Log($"failed to find entity {index} to update");
            }
        }

        // @Update [Group(Features.StateTracking), InterceptIn(nameof(Incoming.RoomUserTyping))]
        private void HandleEntityTyping(InterceptArgs e)
        {
            if (!roomManager.IsInRoom) return;

            int index = e.Packet.ReadInt();

            if (TryGetEntityByIndex(index, out Entity entity))
            {
                bool wasTyping = entity.IsTyping;
                entity.IsTyping = e.Packet.ReadInt() != 0;
                OnEntityTyping(entity, wasTyping);
            }
            else
            {
                DebugUtil.Log($"failed to find entity {index} to update");
            }
        }
    }
}
