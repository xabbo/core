using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Xabbo.Core.Game;

public interface IRoom
{
    /// <summary>
    /// Gets the ID of this room.
    /// </summary>
    Id Id { get; }

    /// <summary>
    /// Gets the room data. The client should request room data before entering a room, however
    /// this may be null during room teleport transitions before the game client loads
    /// </summary>
    IRoomData? Data { get; }

    /// <summary>
    /// Gets the model of this room.
    /// </summary>
    string? Model { get; }

    /// <summary>
    /// Gets the floor pattern of the room.
    /// </summary>
    string? FloorPattern { get; }

    /// <summary>
    /// Gets the wallpaper pattern of the room.
    /// </summary>
    string? Wallpaper { get; }

    /// <summary>
    /// Gets the landscape pattern of the room.
    /// </summary>
    string? Landscape { get; }

    /// <summary>
    /// Gets the location of the entry tile.
    /// </summary>
    Tile Entry { get; }

    /// <summary>
    /// Gets the direction of the entry tile.
    /// </summary>
    int EntryDirection { get; }

    /// <summary>
    /// Gets the floor plan of the room.
    /// </summary>
    IFloorPlan FloorPlan { get; }

    /// <summary>
    /// Gets the heightmap of the room.
    /// </summary>
    IHeightmap Heightmap { get; }

    /// <summary>
    /// Gets if the walls are hidden.
    /// </summary>
    bool HideWalls { get; }
    /// <summary>
    /// Gets the wall thickness.
    /// </summary>
    Thickness WallThickness { get; }

    /// <summary>
    /// Gets the floor thickness.
    /// </summary>
    Thickness FloorThickness { get; }

    #region - Furni -
    /// <summary>
    /// Gets the furni in this room.
    /// </summary>
    IEnumerable<IFurni> Furni => FloorItems.Concat<IFurni>(WallItems);

    /// <summary>
    /// Gets the floor items in this room.
    /// </summary>
    IEnumerable<IFloorItem> FloorItems { get; }

    /// <summary>
    /// Gets the wall items in this room.
    /// </summary>
    IEnumerable<IWallItem> WallItems { get; }

    /// <summary>
    /// Gets if a floor item with the specified ID exists in this room.
    /// </summary>
    bool HasFloorItem(Id id);
    /// <summary>
    /// Gets if a wall item with the specified ID exists in this room.
    /// </summary>
    bool HasWallItem(Id id);

    /// <summary>
    /// Gets the furni of the specified type with the specified ID if it exists.
    /// </summary>
    IFurni? GetFurni(ItemType type, Id id);

    /// <summary>
    /// Gets the floor item with the specified ID if it exists.
    /// </summary>
    IFloorItem? GetFloorItem(Id id);

    /// <summary>
    /// Gets the wall item with the specified ID if it exists.
    /// </summary>
    IWallItem? GetWallItem(Id id);
    #endregion

    #region - Avatars -
    /// <summary>
    /// Gets the avatars in this room.
    /// </summary>
    IEnumerable<IAvatar> Avatars { get; }

    /// <summary>
    /// Gets the users in this room.
    /// </summary>
    IEnumerable<IUser> Users => Avatars.OfType<IUser>();

    /// <summary>
    /// Gets the pets in this room.
    /// </summary>
    IEnumerable<IPet> Pets => Avatars.OfType<IPet>();

    /// <summary>
    /// Gets the bots in this room.
    /// </summary>
    IEnumerable<IBot> Bots => Avatars.OfType<IBot>();

    /// <summary>
    /// Gets the avatar with the specified index if it exists.
    /// </summary>
    TAvatar? GetAvatar<TAvatar>(int index) where TAvatar : IAvatar;

    /// <summary>
    /// Gets the avatar with the specified name if it exists.
    /// </summary>
    TAvatar? GetAvatar<TAvatar>(string name) where TAvatar : IAvatar;

    /// <summary>
    /// Gets the avatar with the specified ID if it exists.
    /// </summary>
    TAvatar? GetAvatarById<TAvatar>(Id id) where TAvatar : IAvatar;

    bool TryGetAvatarByIndex<TAvatar>(int index, [NotNullWhen(true)] out TAvatar? avatar) where TAvatar : IAvatar;
    bool TryGetAvatarById<TAvatar>(Id id, [NotNullWhen(true)] out TAvatar? avatar) where TAvatar : IAvatar;
    bool TryGetAvatarByName<TAvatar>(string name, [NotNullWhen(true)] out TAvatar? avatar) where TAvatar : IAvatar;

    bool TryGetUserByIndex(int index, [NotNullWhen(true)] out IUser? user) => TryGetAvatarByIndex(index, out user);
    bool TryGetUserById(Id id, [NotNullWhen(true)] out IUser? user) => TryGetAvatarById(id, out user);
    bool TryGetUserByName(string name, [NotNullWhen(true)] out IUser? user) => TryGetAvatarByName(name, out user);

    bool TryGetPetByIndex(int index, [NotNullWhen(true)] out IPet? pet) => TryGetAvatarByIndex(index, out pet);
    bool TryGetPetById(Id id, [NotNullWhen(true)] out IPet? pet) => TryGetAvatarById(id, out pet);
    bool TryGetPetByName(string name, [NotNullWhen(true)] out IPet? pet) => TryGetAvatarByName(name, out pet);

    bool TryGetBotByIndex(int index, [NotNullWhen(true)] out IBot? bot) => TryGetAvatarByIndex(index, out bot);
    bool TryGetBotById(Id id, [NotNullWhen(true)] out IBot? bot) => TryGetAvatarById(id, out bot);
    bool TryGetBotByName(string name, [NotNullWhen(true)] out IBot? bot) => TryGetAvatarByName(name, out bot);
    #endregion

    /// <summary>
    /// Finds locations within the specified area where a furniture of the specified size can likely
    /// be placed successfully.
    /// </summary>
    /// <param name="area">The area to place furniture inside.</param>
    /// <param name="size">The size of the furniture to place.</param>
    /// <param name="allowEntryTile">Whether to allow placement on the room entry tile.</param>
    /// <returns>Locations where a furni of the specified size can likely be placed successfully.</returns>
    IEnumerable<Point> FindPlaceablePoints(Area area, Point size, bool allowEntryTile);

    /// <summary>
    /// Attempts to find a location within the specified area where a furniture of the specified
    /// size can likely be placed successfully.
    /// </summary>
    /// <param name="area">The area to place furniture inside.</param>
    /// <param name="size">The size of the furniture to place.</param>
    /// <param name="allowEntryTile">Whether to allow placement on the room entry tile.</param>
    /// <returns>A point where a furni of the specified size can likely be placed successfully.</returns>
    Point? FindPlaceablePoint(Area area, Point size, bool allowEntryTile = false);

    /// <summary>
    /// Attempts to find a location where a furniture of the specified size can likely be placed
    /// successfully.
    /// </summary>
    /// <param name="size">The size of the furniture to place.</param>
    /// <param name="allowEntryTile">Whether to allow placement on the room entry tile.</param>
    /// <returns>A point where a furni of the specified size can likely be placed successfully.</returns>
    Point? FindPlaceablePoint(Point size, bool allowEntryTile = false);
}
