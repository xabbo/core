using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Core.Events;
using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    [Dependencies(typeof(RoomManager))]
    public class EntityManager : XabboComponent
    {
        public enum Features { Tracking, StateTracking }

        private RoomManager roomManager;

        private readonly ConcurrentDictionary<int, Entity> entities = new ConcurrentDictionary<int, Entity>();

        public IEnumerable<IEntity> Entities => entities.Select(x => x.Value);
        public IEnumerable<IRoomUser> Users => Entities.OfType<RoomUser>();
        public IEnumerable<IPet> Pets => Entities.OfType<Pet>();
        public IEnumerable<IBot> Bots => Entities.OfType<Bot>();

        public T GetEntityByIndex<T>(int index) where T : class, IEntity => GetEntityByIndex(index) as T;
        public T GetEntity<T>(int id) where T : IEntity
            => Entities.OfType<T>().FirstOrDefault(e => e.Id == id);
        public T GetEntity<T>(string name) where T : IEntity
            => Entities.OfType<T>().FirstOrDefault(e => e.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        public IEntity GetEntityByIndex(int index) => entities.TryGetValue(index, out Entity e) ? e : null;
        public IEntity GetEntity(int id) => Entities.FirstOrDefault(e => e.Id == id);
        public IEntity GetEntity(string name) => Entities.FirstOrDefault(e => e.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        public bool TryGetEntityByIndex(int index, out IEntity entity)
        {
            entity = null;
            if (entities.TryGetValue(index, out Entity result))
                entity = result;
            return entity != null;
        }
        public bool TryGetEntityByIndex<T>(int index, out T entity) where T : IEntity
        {
            if (entities.TryGetValue(index, out Entity e))
            {
                entity = (T)(IEntity)e;
                return true;
            }
            else
            {
                entity = default(T);
                return false;
            }
        }
        public bool TryGetEntity<T>(int id, out T entity) where T : IEntity
            => (entity = GetEntity<T>(id)) != null;
        public bool TryGetEntity<T>(string name, out T entity) where T : IEntity
            => (entity = GetEntity<T>(name)) != null;

        #region - Events -
        public event EventHandler<EntityEventArgs> EntityAdded;
        public event EventHandler<EntitiesEventArgs> EntitiesAdded;
        public event EventHandler<EntityEventArgs> EntityUpdated;
        public event EventHandler<EntitiesEventArgs> EntitiesUpdated;
        public event EventHandler<EntitySlideEventArgs> EntitySlide;
        public event EventHandler<UserDataUpdatedEventArgs> UserDataUpdated;
        public event EventHandler<EntityNameChangedEventArgs> EntityNameChanged;
        public event EventHandler<EntityIdleEventArgs> EntityIdle;
        public event EventHandler<EntityDanceEventArgs> EntityDance;
        public event EventHandler<EntityHandItemEventArgs> EntityHandItem;
        public event EventHandler<EntityEffectEventArgs> EntityEffect;
        public event EventHandler<EntityActionEventArgs> EntityAction;
        public event EventHandler<EntityTypingEventArgs> EntityTyping;
        public event EventHandler<EntityEventArgs> EntityRemoved;

        protected virtual void OnEntityAdded(IEntity entity)
            => EntityAdded?.Invoke(this, new EntityEventArgs(entity));
        protected virtual void OnEntitiesAdded(IEnumerable<IEntity> entities)
            => EntitiesAdded?.Invoke(this, new EntitiesEventArgs(entities));
        protected virtual void OnEntityUpdated(IEntity entity)
            => EntityUpdated?.Invoke(this, new EntityEventArgs(entity));
        protected virtual void OnEntitiesUpdated(IEnumerable<IEntity> entities)
            => EntitiesUpdated?.Invoke(this, new EntitiesEventArgs(entities));
        protected virtual void OnEntitySlide(IEntity entity, ITile previousTile)
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

        [Receive("RoomUsers")]
        protected void HandleRoomUsers(Packet packet)
        {
            if (!roomManager.IsLoadingRoom && !roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            var newEntities = new List<Entity>();

            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
            {
                var entity = Entity.Parse(packet);
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

        [Receive("RoomUserRemove")]
        protected void HandleEntityRemove(Packet packet)
        {
            if (!roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            int index = int.Parse(packet.ReadString());
            if (entities.TryRemove(index, out Entity entity))
            {
                OnEntityRemoved(entity);
            }
            else
            {
                DebugUtil.Log($"failed to remove entity {index}");
            }
        }

        [Group(Features.Tracking), Receive("RoomUserStatus")]
        protected void HandleEntityUpdate(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

            var updatedEntities = new List<IEntity>();

            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
            {
                var entityUpdate = EntityUpdate.Parse(packet);
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

        [Group(Features.Tracking), Receive("ObjectOnRoller")]
        protected void HandleObjectOnRoller(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

            var rollerUpdate = RollerUpdate.Parse(packet);

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

        [Group(Features.StateTracking), Receive("RoomUserData")]
        protected void HandleUserDataUpdated(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

            int index = packet.ReadInt();
            if (TryGetEntityByIndex(index, out RoomUser user))
            {
                string previousFigure = user.Figure;
                Gender previousGender = user.Gender;
                string previousMotto = user.Motto;
                int previousAchievementScore = user.AchievementScore;

                user.Figure = packet.ReadString();
                user.Gender = H.ToGender(packet.ReadString());
                user.Motto = packet.ReadString();
                user.AchievementScore = packet.ReadInt();

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

        [Group(Features.StateTracking), Receive("RoomUserNameChanged")]
        protected void HandleRoomUserNameChanged(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

            int id = packet.ReadInt();
            int index = packet.ReadInt();
            string newName = packet.ReadString();

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

        [Group(Features.StateTracking), Receive("RoomUnitIdle")]
        protected void HandleEntityIdle(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

            int index = packet.ReadInt();
            if (TryGetEntityByIndex(index, out Entity entity))
            {
                bool wasIdle = entity.IsIdle;
                entity.IsIdle = packet.ReadBool();
                OnEntityIdle(entity, wasIdle);
            }
            else
            {
                DebugUtil.Log($"failed to find entity {index} to update");
            }
        }

        [Group(Features.StateTracking), Receive("RoomUserDance")]
        protected void HandleEntityDance(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

            int index = packet.ReadInt();
            var entity = GetEntityByIndex(index) as Entity;
            if (entity == null)
            {
                DebugUtil.Log($"failed to find entity {index} to update");
                return;
            }

            int previousDance = entity.Dance;
            entity.Dance = packet.ReadInt();

            OnEntityDance(entity, previousDance);
        }

        [Group(Features.StateTracking), Receive("RoomUserAction")]
        protected void HandleEntityAction(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

            int index = packet.ReadInt();
            if (TryGetEntityByIndex(index, out Entity entity))
            {
                OnEntityAction(entity, (Actions)packet.ReadInt());
            }
            else
            {
                DebugUtil.Log($"failed to find entity {index} to update");
            }
        }

        [Group(Features.StateTracking), Receive("RoomUserHandItem")]
        protected void HandleEntityHandItem(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

            int index = packet.ReadInt();
            if (TryGetEntityByIndex(index, out Entity entity))
            {
                int previousItem = entity.HandItem;
                entity.HandItem = packet.ReadInt();
                OnEntityHandItem(entity, previousItem);
            }
            else
            {
                DebugUtil.Log($"failed to find entity {index} to update");
            }
        }

        [Group(Features.StateTracking), Receive("RoomUserEffect")]
        protected void HandleEntityEffect(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

            int index = packet.ReadInt();
            if (TryGetEntityByIndex(index, out Entity entity))
            {
                int previousEffect = entity.Effect;
                entity.Effect = packet.ReadInt();
                OnEntityEffect(entity, previousEffect);
            }
            else
            {
                DebugUtil.Log($"failed to find entity {index} to update");
            }
        }

        [Group(Features.StateTracking), Receive("RoomUserTyping")]
        protected void HandleEntityTyping(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

            int index = packet.ReadInt();

            if (TryGetEntityByIndex(index, out Entity entity))
            {
                bool wasTyping = entity.IsTyping;
                entity.IsTyping = packet.ReadInt() != 0;
                OnEntityTyping(entity, wasTyping);
            }
            else
            {
                DebugUtil.Log($"failed to find entity {index} to update");
            }
        }
    }
}
