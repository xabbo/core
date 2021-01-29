using System;

using Xabbo.Core.Game;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public interface IEntity : IPacketData
    {
        /// <summary>
        /// Gets if the entity has been removed from the room.
        /// </summary>
        bool IsRemoved { get; }

        /// <summary>
        /// Gets if the entity is hidden client-side by the <see cref="EntityManager" />.
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        EntityType Type { get; }

        /// <summary>
        /// Gets the ID of the entity.
        /// </summary>
        long Id { get; }

        /// <summary>
        /// Gets the index of the entity.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets the name of the entity.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the motto of the entity.
        /// </summary>
        string Motto { get; }

        /// <summary>
        /// Gets the figure string of the entity.
        /// </summary>
        string Figure { get; }

        /// <summary>
        /// Gets the location of the entity.
        /// </summary>
        Tile Location { get; }

        /// <summary>
        /// Gets the X coordinate of the entity.
        /// </summary>
        int X { get; }

        /// <summary>
        /// Gets the Y coordinate of the entity.
        /// </summary>
        int Y { get; }

        /// <summary>
        /// Gets the XY coordinates of the entity.
        /// </summary>
        (int X, int Y) XY { get; }

        /// <summary>
        /// Gets the Z coordinate of the entity.
        /// </summary>
        float Z { get; }

        /// <summary>
        /// Gets the XYZ coordinates of the entity.
        /// </summary>
        (int X, int Y, float Z) XYZ { get; }

        /// <summary>
        /// Gets the direction of the entity.
        /// </summary>
        int Direction { get; }

        /// <summary>
        /// Gets the current dance of the entity.
        /// </summary>
        int Dance { get; }
        
        /// <summary>
        /// Gets if the entity is idle or not.
        /// </summary>
        bool IsIdle { get; }

        /// <summary>
        /// Gets if the entity is typing or not.
        /// </summary>
        bool IsTyping { get; }

        /// <summary>
        /// Gets the hand item the entity is currently holding.
        /// </summary>
        int HandItem { get; }

        /// <summary>
        /// Gets the current effect of the entity.
        /// </summary>
        int Effect { get; }

        /// <summary>
        /// Gets the current update of the entity.
        /// </summary>
        IEntityStatusUpdate? CurrentUpdate { get; }

        /// <summary>
        /// Gets the previous update of the entity.
        /// </summary>
        IEntityStatusUpdate? PreviousUpdate { get; }
    }
}
