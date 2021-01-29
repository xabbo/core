using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Core.Events;
using Xabbo.Core.Messages;

namespace Xabbo.Core.Game
{
    /// <summary>
    /// Manages and tracks entities in the room.
    /// </summary>
    public class EntityManager : GameStateManager
    {
        private readonly RoomManager _roomManager;
        private readonly ConcurrentDictionary<int, Entity> _entities = new();

        /// <summary>
        /// Gets the entities in the room.
        /// </summary>
        public IEnumerable<IEntity> Entities => _entities.Select(x => x.Value);
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
        public T? GetEntityByIndex<T>(int index) where T : class, IEntity => GetEntityByIndex(index) as T;
        /// <summary>
        /// Gets the entity of the specified type with the specified ID, or <c>null</c> if it does not exist.
        /// </summary>
        public T? GetEntity<T>(long id) where T : IEntity
            => Entities.OfType<T>().FirstOrDefault(e => e.Id == id);
        /// <summary>
        /// Gets the entity of the specified type with the specified name, or <c>null</c> if it does not exist.
        /// </summary>
        public T? GetEntity<T>(string name) where T : IEntity
            => Entities.OfType<T>().FirstOrDefault(e => e.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        /// <summary>
        /// Gets the entity with the specified index, or <c>null</c> if it does not exist.
        /// </summary>
        public IEntity? GetEntityByIndex(int index) => _entities.TryGetValue(index, out Entity? e) ? e : null;
        /// <summary>
        /// Gets the entity with the specified ID, or <c>null</c> if it does not exist.
        /// </summary>
        public IEntity? GetEntity(long id) => Entities.FirstOrDefault(e => e.Id == id);
        /// <summary>
        /// Gets the entity with the specified name, or <c>null</c> if it does not exist.
        /// </summary>
        public IEntity? GetEntity(string name) => Entities.FirstOrDefault(e => e.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        /// <summary>
        /// Attempts to get the entity with the specified index.
        /// </summary>
        public bool TryGetEntityByIndex(int index, out IEntity? entity)
        {
            if (_entities.TryGetValue(index, out Entity? result))
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
        public bool TryGetEntityByIndex<T>(int index, out T? entity) where T : IEntity
        {
            if (_entities.TryGetValue(index, out Entity? e))
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
        public bool TryGetEntity<T>(int id, out T? entity) where T : IEntity
            => (entity = GetEntity<T>(id)) != null;

        /// <summary>
        /// Attempts to get the entity of the specified type with the specified name.
        /// </summary>
        public bool TryGetEntity<T>(string name, out T? entity) where T : IEntity
            => (entity = GetEntity<T>(name)) != null;

        #region - Events -
        /// <summary>
        /// Invoked when an entity has been added to the room.
        /// </summary>
        public event EventHandler<EntityEventArgs>? EntityAdded;
        /// <summary>
        /// Invoked when entities have been added to the room.
        /// </summary>
        public event EventHandler<EntitiesEventArgs>? EntitiesAdded;
        /// <summary>
        /// Invoked when an entity in the room is updated.
        /// </summary>
        public event EventHandler<EntityEventArgs>? EntityUpdated;
        /// <summary>
        /// Invoked when entities in the room are updated.
        /// </summary>
        public event EventHandler<EntitiesEventArgs>? EntitiesUpdated;
        /// <summary>
        /// Invoked when an entity slides along a roller.
        /// </summary>
        public event EventHandler<EntitySlideEventArgs>? EntitySlide;
        /// <summary>
        /// Invoked when a user's figure, motto or achievement score is updated.
        /// </summary>
        public event EventHandler<UserDataUpdatedEventArgs>? UserDataUpdated;
        /// <summary>
        /// Invoked when an entity's name changes.
        /// </summary>
        public event EventHandler<EntityNameChangedEventArgs>? EntityNameChanged;
        /// <summary>
        /// Invoked when an entity's idle status updates.
        /// </summary>
        public event EventHandler<EntityIdleEventArgs>? EntityIdle;
        /// <summary>
        /// Invoked when an entity's dance updates.
        /// </summary>
        public event EventHandler<EntityDanceEventArgs>? EntityDance;
        /// <summary>
        /// Invoked when an entity's hand item updates.
        /// </summary>
        public event EventHandler<EntityHandItemEventArgs>? EntityHandItem;
        /// <summary>
        /// Invoked when an entity's effect updates.
        /// </summary>
        public event EventHandler<EntityEffectEventArgs>? EntityEffect;
        /// <summary>
        /// Invoked when an entity performs an action.
        /// </summary>
        public event EventHandler<EntityExpressionEventArgs>? EntityAction;
        /// <summary>
        /// Invoked when an entity's typing status updates.
        /// </summary>
        public event EventHandler<EntityTypingEventArgs>? EntityTyping;
        /// <summary>
        /// Invoked when an entity is removed from the room.
        /// </summary>
        public event EventHandler<EntityEventArgs>? EntityRemoved;

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
        protected virtual void OnEntityExpression(IEntity entity, Expressions action)
            => EntityAction?.Invoke(this, new EntityExpressionEventArgs(entity, action));
        protected virtual void OnEntityTyping(IEntity entity, bool wasTyping)
            => EntityTyping?.Invoke(this, new EntityTypingEventArgs(entity, wasTyping));
        protected virtual void OnEntityRemoved(IEntity entity)
            => EntityRemoved?.Invoke(this, new EntityEventArgs(entity));
        #endregion

        public EntityManager(IInterceptor interceptor, RoomManager roomManager)
            : base(interceptor)
        {
            _roomManager = roomManager;
            _roomManager.Left += Room_Left;
        }

        private void Room_Left(object? sender, EventArgs e)
        {
            DebugUtil.Log("clearing entities");

            _entities.Clear();
        }

        /// <summary>
        /// Hides the entity client-side.
        /// </summary>
        public void Hide(IEntity entity)
        {
            if (entity is not Entity e)
                return;

            if (!e.IsHidden)
            {
                e.IsHidden = true;
                SendLocalAsync(In.UserLoggedOut, e.Index);
            }
        }

        /// <summary>
        /// Hides the entities client-side.
        /// </summary>
        /// <param name="entities"></param>
        public void Hide(IEnumerable<IEntity> entities)
        {
            foreach (Entity e in entities.OfType<Entity>())
            {
                
            }
        }

        /// <summary>
        /// Shows the entity after hiding it client-side.
        /// </summary>
        /// <param name="entity"></param>
        public void Show(IEntity entity)
        {
            if (entity is not Entity e)
                return;

            if (e.IsHidden)
            {
                e.IsHidden = false;
                SendLocalAsync(In.UsersInRoom, 1, e);
            }
        }

        /// <summary>
        /// Shows the entities after hiding them client-side.
        /// </summary>
        public void Show(IEnumerable<IEntity> entities)
        {
            List<IEntity> shown = new();
            foreach (Entity entity in entities.OfType<Entity>())
            {
                if (entity.IsHidden)
                {
                    entity.IsHidden = false;
                    shown.Add(entity);
                }
            }

            SendLocalAsync(In.UsersInRoom, shown);
        }

        [InterceptIn(nameof(Incoming.UsersInRoom))]
        private void HandleUsersInRoom(InterceptArgs e)
        {
            if (!_roomManager.IsLoadingRoom && !_roomManager.IsInRoom)
                return;

            List<Entity> newEntities = new List<Entity>();

            int n = e.Packet.ReadShort();
            for (int i = 0; i < n; i++)
            {
                Entity entity = Entity.Parse(e.Packet);
                if (_entities.TryAdd(entity.Index, entity))
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

        [InterceptIn(nameof(Incoming.UserLoggedOut))]
        private void HandleUserLoggedOut(InterceptArgs e)
        {
            if (!_roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            int index = e.Packet.ReadInt();
            if (_entities.TryRemove(index, out Entity? entity))
            {
                OnEntityRemoved(entity);
            }
            else
            {
                DebugUtil.Log($"failed to remove entity {index}");
            }
        }

        [InterceptIn(nameof(Incoming.Status))]
        private void HandleStatus(InterceptArgs e)
        {
            if (!_roomManager.IsInRoom) return;

            var updatedEntities = new List<IEntity>();

            int n = e.Packet.ReadShort();
            for (int i = 0; i < n; i++)
            {
                EntityStatusUpdate update = EntityStatusUpdate.Parse(e.Packet);
                if (!_entities.TryGetValue(update.Index, out Entity? entity))
                {
                    DebugUtil.Log($"failed to find entity {update.Index} to update");
                    continue;
                }

                entity.Update(update);
                updatedEntities.Add(entity);

                OnEntityUpdated(entity);
            }

            OnEntitiesUpdated(updatedEntities);
        }

        [InterceptIn(nameof(Incoming.QueueMoveUpdate))]
        private void HandleQueueMoveUpdate(InterceptArgs e)
        {
            if (!_roomManager.IsInRoom) return;

            var rollerUpdate = RollerUpdate.Parse(e.Packet);

            if (rollerUpdate.Type == RollerUpdateType.MovingEntity ||
                rollerUpdate.Type == RollerUpdateType.StationaryEntity)
            {
                if (_entities.TryGetValue(rollerUpdate.EntityIndex, out Entity? entity))
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

        [InterceptIn(nameof(Incoming.UpdateAvatar))]
        private void HandleUpdateAvatar(InterceptArgs e)
        {
            if (!_roomManager.IsInRoom) return;

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

        [InterceptIn(nameof(Incoming.UserNameChanged))] // @Check
        private void HandleUserNameChanged(InterceptArgs e)
        {
            if (!_roomManager.IsInRoom) return;

            long id = e.Packet.ReadLong();
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

        [InterceptIn(nameof(Incoming.RoomAvatarSleeping))]
        private void HandleRoomAvatarSleeping(InterceptArgs e)
        {
            if (!_roomManager.IsInRoom) return;

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

        [InterceptIn(nameof(Incoming.RoomDance))]
        private void HandleRoomDance(InterceptArgs e)
        {
            if (!_roomManager.IsInRoom) return;

            int index = e.Packet.ReadInt();
            if (!TryGetEntityByIndex(index, out Entity entity))
            {
                DebugUtil.Log($"failed to find entity {index} to update");
                return;
            }

            int previousDance = entity.Dance;
            entity.Dance = e.Packet.ReadInt();

            OnEntityDance(entity, previousDance);
        }

        [InterceptIn(nameof(Incoming.RoomExpression))]
        private void HandleRoomExpression(InterceptArgs e)
        {
            if (!_roomManager.IsInRoom) return;

            int index = e.Packet.ReadInt();
            if (!TryGetEntityByIndex(index, out Entity entity))
            {
                DebugUtil.Log($"failed to find entity {index} to update");
                return;
            }

            OnEntityExpression(entity, (Expressions)e.Packet.ReadInt());
        }

        [InterceptIn(nameof(Incoming.RoomCarryObject))]
        private void HandleRoomCarryObject(InterceptArgs e)
        {
            if (!_roomManager.IsInRoom) return;

            int index = e.Packet.ReadInt();
            if (!TryGetEntityByIndex(index, out Entity entity))
            {
                DebugUtil.Log($"failed to find entity {index} to update");
                return;
            }

            int previousItem = entity.HandItem;
            entity.HandItem = e.Packet.ReadInt();
            OnEntityHandItem(entity, previousItem);
        }

        [InterceptIn(nameof(Incoming.RoomAvatarEffect))]
        private void HandleRoomAvatarEffect(InterceptArgs e)
        {
            if (!_roomManager.IsInRoom) return;

            int index = e.Packet.ReadInt();
            if (!TryGetEntityByIndex(index, out Entity entity))
            {
                DebugUtil.Log($"failed to find entity {index} to update");
                return;
            }

            int previousEffect = entity.Effect;
            entity.Effect = e.Packet.ReadInt();
            OnEntityEffect(entity, previousEffect);

            // + int delay
        }

        [InterceptIn(nameof(Incoming.UserTypingStatusChange))]
        private void HandleUserTypingStatusChange(InterceptArgs e)
        {
            if (!_roomManager.IsInRoom) return;

            int index = e.Packet.ReadInt();

            if (!TryGetEntityByIndex(index, out Entity entity))
            {
                DebugUtil.Log($"failed to find entity {index} to update");
                return;
            }

            bool wasTyping = entity.IsTyping;
            entity.IsTyping = e.Packet.ReadInt() != 0;
            OnEntityTyping(entity, wasTyping);
        }
    }
}
