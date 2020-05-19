﻿using System;
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
        public enum Features { EntityUpdates, StateTracking }

        private RoomManager roomManager;

        private readonly ConcurrentDictionary<int, Entity> entities = new ConcurrentDictionary<int, Entity>();

        public IEnumerable<Entity> Entities => entities.Select(x => x.Value);
        public IEnumerable<RoomUser> Users => Entities.OfType<RoomUser>();
        public IEnumerable<Pet> Pets => Entities.OfType<Pet>();
        public IEnumerable<PrivateBot> UserBots => Entities.OfType<PrivateBot>();
        public IEnumerable<PublicBot> PublicBots => Entities.OfType<PublicBot>();

        public Entity GetEntity(int index) => entities.TryGetValue(index, out Entity e) ? e : null;
        public Entity GetEntityById(int id) => Entities.FirstOrDefault(e => e.Id == id);
        public Entity GetEntityByName(string name) => Entities.FirstOrDefault(e => e.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        public T GetEntity<T>(int index) where T : Entity => GetEntity(index) as T;
        public T GetEntityById<T>(int id) where T : Entity
            => Entities.OfType<T>().FirstOrDefault(e => e.Id == id);
        public T GetEntityByName<T>(string name) where T : Entity
            => Entities.OfType<T>().FirstOrDefault(e => e.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        public bool TryGetEntity(int index, out Entity entity) => entities.TryGetValue(index, out entity);
        public bool TryGetEntity<T>(int index, out T entity) where T : Entity
        {
            if (entities.TryGetValue(index, out Entity e))
            {
                entity = (T)e;
                return true;
            }
            else
            {
                entity = null;
                return false;
            }
        }

        public bool TryGetEntityById<T>(int id, out T entity) where T : Entity
            => (entity = GetEntityById<T>(id)) != null;
        public bool TryGetEntityByName<T>(string name, out T entity) where T : Entity
            => (entity = GetEntityByName<T>(name)) != null;

        #region - Events -
        public event EventHandler<EntityEventArgs> EntityAdded;
        public event EventHandler<EntitiesEventArgs> EntitiesAdded;
        public event EventHandler<EntityEventArgs> EntityUpdated;
        public event EventHandler<EntitiesEventArgs> EntitiesUpdated;
        public event EventHandler<EntitySlideEventArgs> EntitySlide;
        public event EventHandler<UserDataUpdatedEventArgs> UserDataUpdated;
        public event EventHandler<EntityIdleEventArgs> EntityIdle;
        public event EventHandler<EntityDanceEventArgs> EntityDance;
        public event EventHandler<EntityHandItemEventArgs> EntityHandItem;
        public event EventHandler<EntityEffectEventArgs> EntityEffect;
        public event EventHandler<EntityActionEventArgs> EntityAction;
        public event EventHandler<EntityTypingEventArgs> EntityTyping;
        public event EventHandler<EntityEventArgs> EntityRemoved;

        protected virtual void OnEntityAdded(Entity entity)
            => EntityAdded?.Invoke(this, new EntityEventArgs(entity));
        protected virtual void OnEntitiesAdded(IEnumerable<Entity> entities)
            => EntitiesAdded?.Invoke(this, new EntitiesEventArgs(entities));
        protected virtual void OnEntityUpdated(Entity entity)
            => EntityUpdated?.Invoke(this, new EntityEventArgs(entity));
        protected virtual void OnEntitiesUpdated(IEnumerable<Entity> entities)
            => EntitiesUpdated?.Invoke(this, new EntitiesEventArgs(entities));
        protected virtual void OnEntitySlide(Entity entity, Tile previousTile)
            => EntitySlide?.Invoke(this, new EntitySlideEventArgs(entity, previousTile));
        protected virtual void OnUserDataUpdated(RoomUser user,
            string previousFigure, Gender previousGender,
            string previousMotto, int previousAchievementScore)
            => UserDataUpdated?.Invoke(this, new UserDataUpdatedEventArgs(
                user, previousFigure, previousGender,
                previousMotto, previousAchievementScore
            ));
        protected virtual void OnEntityIdle(Entity entity, bool wasIdle)
            => EntityIdle?.Invoke(this, new EntityIdleEventArgs(entity, wasIdle));
        protected virtual void OnEntityDance(Entity entity, int previousDance)
            => EntityDance?.Invoke(this, new EntityDanceEventArgs(entity, previousDance));
        protected virtual void OnEntityHandItem(Entity entity, int previousItem)
            => EntityHandItem?.Invoke(this, new EntityHandItemEventArgs(entity, previousItem));
        protected virtual void OnEntityEffect(Entity entity, int previousEffect)
            => EntityEffect?.Invoke(this, new EntityEffectEventArgs(entity, previousEffect));
        protected virtual void OnEntityAction(Entity entity, Actions action)
            => EntityAction?.Invoke(this, new EntityActionEventArgs(entity, action));
        protected virtual void OnEntityTyping(Entity entity, bool wasTyping)
            => EntityTyping?.Invoke(this, new EntityTypingEventArgs(entity, wasTyping));
        protected virtual void OnEntityRemoved(Entity entity)
            => EntityRemoved?.Invoke(this, new EntityEventArgs(entity));
        #endregion

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

            int n = packet.ReadInteger();
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

        [Group(Features.EntityUpdates), Receive("RoomUserStatus")]
        protected void HandleEntityUpdate(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

            var updatedEntities = new List<Entity>();

            int n = packet.ReadInteger();
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

        [Group(Features.EntityUpdates), Receive("ObjectOnRoller")]
        protected void HandleObjectOnRoller(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

            var rollerUpdate = RollerUpdate.Parse(packet);

            if (rollerUpdate.Type == RollerUpdateType.MovingEntity ||
                rollerUpdate.Type == RollerUpdateType.StationaryEntity)
            {
                if (entities.TryGetValue(rollerUpdate.EntityIndex, out Entity entity))
                {
                    var previousTile = entity.Tile;
                    entity.Tile = new Tile(rollerUpdate.TargetX, rollerUpdate.TargetY, rollerUpdate.EntityTargetZ);

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

            int index = packet.ReadInteger();
            if (TryGetEntity(index, out RoomUser user))
            {
                string previousFigure = user.Figure;
                Gender previousGender = user.Gender;
                string previousMotto = user.Motto;
                int previousAchievementScore = user.AchievementScore;

                user.Figure = packet.ReadString();
                user.Gender = H.ToGender(packet.ReadString());
                user.Motto = packet.ReadString();
                user.AchievementScore = packet.ReadInteger();

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

        [Group(Features.StateTracking), Receive("RoomUnitIdle")]
        protected void HandleEntityIdle(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

            int index = packet.ReadInteger();
            if (TryGetEntity(index, out Entity entity))
            {
                bool wasIdle = entity.IsIdle;
                entity.IsIdle = packet.ReadBoolean();
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

            int index = packet.ReadInteger();
            var entity = GetEntity(index);
            if (entity == null)
            {
                DebugUtil.Log($"failed to find entity {index} to update");
                return;
            }

            int previousDance = entity.Dance;
            entity.Dance = packet.ReadInteger();

            OnEntityDance(entity, previousDance);
        }

        [Group(Features.StateTracking), Receive("RoomUserAction")]
        protected void HandleEntityAction(Packet packet)
        {
            if (!roomManager.IsInRoom) return;

            int index = packet.ReadInteger();
            if (TryGetEntity(index, out Entity entity))
            {
                OnEntityAction(entity, (Actions)packet.ReadInteger());
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

            int index = packet.ReadInteger();
            if (TryGetEntity(index, out Entity entity))
            {
                int previousItem = entity.HandItem;
                entity.HandItem = packet.ReadInteger();
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

            int index = packet.ReadInteger();
            if (TryGetEntity(index, out Entity entity))
            {
                int previousEffect = entity.Effect;
                entity.Effect = packet.ReadInteger();
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

            int index = packet.ReadInteger();

            if (TryGetEntity(index, out Entity entity))
            {
                bool wasTyping = entity.IsTyping;
                entity.IsTyping = packet.ReadInteger() != 0;
                OnEntityTyping(entity, wasTyping);
            }
            else
            {
                DebugUtil.Log($"failed to find entity {index} to update");
            }
        }
    }
}
