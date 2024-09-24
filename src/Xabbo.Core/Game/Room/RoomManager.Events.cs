using System;
using Microsoft.Extensions.Logging;

using Xabbo.Core.Events;

namespace Xabbo.Core.Game;

public sealed partial class RoomManager
{
    #region - Room events -
    /// <summary>
    /// Invoked when the user enters the queue to a room.
    /// </summary>
    public event Action? EnteredQueue;

    /// <summary>
    /// Invoked when the user's position in the queue is updated.
    /// </summary>
    public event Action? QueuePositionUpdated;

    /// <summary>
    /// Invoked when the user enters a room and begins loading the room state.
    /// </summary>
    public event Action? Entering;

    /// <summary>
    /// Invoked after the user has entered the room and the room state is fully loaded.
    /// </summary>
    public event Action<RoomEventArgs>? Entered;

    /// <summary>
    /// Invoked when the room data is updated.
    /// </summary>
    public event Action<RoomDataEventArgs>? RoomDataUpdated;

    /// <summary>
    /// Invoked when the user leaves a room.
    /// </summary>
    public event Action? Left;

    /// <summary>
    /// Invoked when the user is kicked from the room.
    /// The user still remains in the room at this point until their avatar leaves through the door.
    /// </summary>
    public event Action? Kicked;

    /// <summary>
    /// Invoked when the user's rights to the room are updated.
    /// </summary>
    public event Action? RightsUpdated;
    #endregion

    #region - Furni events -
    /// <summary>
    /// Invoked when the floor items are loaded.
    /// This may happen multiple times depending on how many items are in the room.
    /// </summary>
    public event Action<FloorItemsEventArgs>? FloorItemsLoaded;

    /// <summary>
    /// Invoked when a floor item is added to the room.
    /// </summary>
    public event Action<FloorItemEventArgs>? FloorItemAdded;

    /// <summary>
    /// Invoked when a floor item is updated.
    /// </summary>
    public event Action<FloorItemUpdatedEventArgs>? FloorItemUpdated;

    /// <summary>
    /// Invoked when a floor item's data is updated.
    /// </summary>
    public event Action<FloorItemDataUpdatedEventArgs>? FloorItemDataUpdated;

    /// <summary>
    /// Invoked when a dice value is updated.
    /// </summary>
    public event Action<DiceUpdatedEventArgs>? DiceUpdated;

    /// <summary>
    /// Invoked when a floor item slides due to a roller or wired update.
    /// </summary>
    public event Action<FloorItemSlideEventArgs>? FloorItemSlide;
    private void OnFloorItemSlide(IFloorItem item, Tile previousTile, Id rollerId)
    {
        Log.LogTrace(
            "Floor item slide. (id:{Id}, rollerId:{RollerId}, {From} -> {To})",
            item.Id, rollerId, previousTile, item.Location
        );
        FloorItemSlide?.Invoke(new FloorItemSlideEventArgs(item, previousTile, rollerId));
    }

    /// <summary>
    /// Invoked when users or furni are moved by wired.
    /// </summary>
    public event Action<WiredMovementsEventArgs>? WiredMovements;

    /// <summary>
    /// Invoked when a floor item is removed from the room.
    /// </summary>
    public event Action<FloorItemEventArgs>? FloorItemRemoved;

    /// <summary>
    /// Invoked when the wall items are loaded.
    /// This may happen multiple times depending on how many items are in the room.
    /// </summary>
    public event Action<WallItemsEventArgs>? WallItemsLoaded;

    /// <summary>
    /// Invoked when a wall item is added to the room.
    /// </summary>
    public event Action<WallItemEventArgs>? WallItemAdded;

    /// <summary>
    /// Invoked when a wall item is udpated.
    /// </summary>
    public event Action<WallItemUpdatedEventArgs>? WallItemUpdated;

    /// <summary>
    /// Invoked when a wall item is removed from the room.
    /// </summary>
    public event Action<WallItemEventArgs>? WallItemRemoved;

    /// <summary>
    /// Invoked when a furni's visibility is toggled using <see cref="HideFurni(IFurni)"/> or <see cref="ShowFurni(IFurni)"/>.
    /// </summary>
    public event Action<FurniEventArgs>? FurniVisibilityToggled;
    #endregion

    #region - Avatar events -
    /// <summary>
    /// Invoked when an avatar has been added to the room.
    /// </summary>
    public event Action<AvatarEventArgs>? AvatarAdded;

    /// <summary>
    /// Invoked when avatars have been added to the room.
    /// </summary>
    public event Action<AvatarsEventArgs>? AvatarsAdded;

    /// <summary>
    /// Invoked when an avatar in the room is updated.
    /// </summary>
    public event Action<AvatarEventArgs>? AvatarUpdated;

    /// <summary>
    /// Invoked when avatars in the room are updated.
    /// </summary>
    public event Action<AvatarsEventArgs>? AvatarsUpdated;

    /// <summary>
    /// Invoked when an avatar slides along a roller.
    /// </summary>
    public event Action<AvatarSlideEventArgs>? AvatarSlide;

    /// <summary>
    /// Invoked when an avatar's figure, gender, motto or achievement score is updated.
    /// </summary>
    public event Action<AvatarDataUpdatedEventArgs>? AvatarDataUpdated;

    /// <summary>
    /// Invoked when an avatar's name changes.
    /// </summary>
    public event Action<AvatarNameChangedEventArgs>? AvatarNameChanged;

    /// <summary>
    /// Invoked when an avatar's idle status updates.
    /// </summary>
    public event Action<AvatarIdleEventArgs>? AvatarIdle;

    /// <summary>
    /// Invoked when an avatar's dance updates.
    /// </summary>
    public event Action<AvatarDanceEventArgs>? AvatarDance;

    /// <summary>
    /// Invoked when an avatar's hand item updates.
    /// </summary>
    public event Action<AvatarHandItemEventArgs>? AvatarHandItem;

    /// <summary>
    /// Invoked when an avatar's effect updates.
    /// </summary>
    public event Action<AvatarEffectEventArgs>? AvatarEffect;

    /// <summary>
    /// Invoked when an avatar performs an action.
    /// </summary>
    public event Action<AvatarActionEventArgs>? AvatarAction;

    /// <summary>
    /// Invoked when an avatar's typing status updates.
    /// </summary>
    public event Action<AvatarTypingEventArgs>? AvatarTyping;

    /// <summary>
    /// Invoked when an avatar is removed from the room.
    /// </summary>
    public event Action<AvatarEventArgs>? AvatarRemoved;

    /// <summary>
    /// Invoked when an avatar in the room talks.
    /// </summary>
    public event Action<AvatarChatEventArgs>? AvatarChat;
    #endregion
}