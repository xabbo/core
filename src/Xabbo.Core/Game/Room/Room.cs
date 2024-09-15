using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Xabbo.Core.Game;

internal class Room : IRoom, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string? propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public Id Id { get; }
    public string Model { get; set; } = null!;

    private RoomData? _roomData;
    public RoomData? Data
    {
        get => _roomData;
        set => Set(ref _roomData, value);
    }
    IRoomData? IRoom.Data => Data;

    private string? _floor;
    public string? Floor
    {
        get => _floor;
        set => Set(ref _floor, value);
    }

    private string? _wallpaper;
    public string? Wallpaper
    {
        get => _wallpaper;
        set => Set(ref _wallpaper, value);
    }

    private string? _landscape;
    public string? Landscape
    {
        get => _landscape;
        set => Set(ref _landscape, value);
    }

    public Tile DoorTile { get; set; }
    public int EntryDirection { get; set; }

    public FloorPlan FloorPlan { get; set; } = null!;
    IFloorPlan IRoom.FloorPlan => FloorPlan;

    public Heightmap Heightmap { get; set; } = null!;
    IHeightmap IRoom.Heightmap => Heightmap;

    public bool HideWalls { get; set; }
    public Thickness WallThickness { get; set; } = Thickness.Normal;
    public Thickness FloorThickness { get; set; } = Thickness.Normal;

    IEnumerable<IFloorItem> IRoom.FloorItems => FloorItems.Select(x => x.Value);
    IEnumerable<IWallItem> IRoom.WallItems => WallItems.Select(x => x.Value);
    IEnumerable<IAvatar> IRoom.Avatars => Avatars.Select(x => x.Value);

    internal ConcurrentDictionary<long, FloorItem> FloorItems { get; } = new();
    internal ConcurrentDictionary<long, WallItem> WallItems { get; } = new();

    internal ConcurrentDictionary<int, Avatar> Avatars { get; } = new();

    public Room(Id id, RoomData? roomData)
    {
        Id = id;
        _roomData = roomData;
    }

    #region - Furni -
    /// <summary>
    /// Gets whether the floor item with the specified ID exists in the room or not.
    /// </summary>
    public bool HasFloorItem(Id itemId) => FloorItems.ContainsKey(itemId);

    /// <summary>
    /// Gets whether the wall item with the specified ID exists in the room or not.
    /// </summary>
    public bool HasWallItem(Id itemId) => WallItems.ContainsKey(itemId);

    /// <summary>
    /// Attempts to get the furni of the specified type with the specified ID and returns <c>true</c> if successful.
    /// </summary>
    public bool TryGetFurni(ItemType type, Id itemId, [NotNullWhen(true)] out IFurni? furni)
        => (furni = GetFurni(type, itemId)) is not null;

    /// <summary>
    /// Attempts to get the floor item with the specified ID and returns <c>true</c> if successful.
    /// </summary>
    public bool TryGetFloorItem(Id itemId, [NotNullWhen(true)] out IFloorItem? item)
        => (item = GetFloorItem(itemId)) is not null;

    /// <summary>
    /// Attempts to get the wall item with the specified ID and returns <c>true</c> if successful.
    /// </summary>
    public bool TryGetWallItem(Id itemId, [NotNullWhen(true)] out IWallItem? item)
        => (item = GetWallItem(itemId)) is not null;

    /// <summary>
    /// Gets the furni of the specified type with the specified ID, or <c>null</c> if it does not exist.
    /// </summary>
    public IFurni? GetFurni(ItemType type, Id itemId)
    {
        return type switch
        {
            ItemType.Floor => GetFloorItem(itemId),
            ItemType.Wall => GetWallItem(itemId),
            _ => throw new InvalidOperationException("Item type must be Floor or Wall")
        };
    }

    /// <summary>
    /// Gets the floor item with the specified ID or <c>null</c> if it does not exist.
    /// </summary>
    public IFloorItem? GetFloorItem(Id itemId) => FloorItems.TryGetValue(itemId, out FloorItem? item) ? item : null;

    /// <summary>
    /// Gets the wall item with the specified ID or <c>null</c> if it does not exist.
    /// </summary>
    public IWallItem? GetWallItem(Id itemId) => WallItems.TryGetValue(itemId, out WallItem? item) ? item : null;
    #endregion

    #region - Avatars -
    public TAvatar? GetAvatar<TAvatar>(int index) where TAvatar : IAvatar
    {
        if (Avatars.TryGetValue(index, out Avatar? avatar) &&
            avatar is TAvatar typedAvatar)
        {
            return typedAvatar;
        }
        else
        {
            return default;
        }
    }

    public TAvatar? GetAvatar<TAvatar>(string name) where TAvatar : IAvatar
    {
        return Avatars
            .Select(x => x.Value)
            .OfType<TAvatar>()
            .FirstOrDefault(avatar => avatar.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public TAvatar? GetAvatarById<TAvatar>(Id id) where TAvatar : IAvatar
    {
        return Avatars
            .Select(x => x.Value)
            .OfType<TAvatar>()
            .FirstOrDefault(avatar => avatar.Id == id);
    }

    public IAvatar? GetAvatar(int index) => Avatars.TryGetValue(index, out Avatar? avatar) ? avatar : null;
    public IUser? GetUser(int index) => Avatars.TryGetValue(index, out Avatar? avatar) ? (avatar as IUser) : null;
    public IUser? GetUserById(Id id) => Avatars.Values.OfType<IUser>().FirstOrDefault(x => x.Id == id);

    public bool TryGetAvatarByIndex(int index, [NotNullWhen(true)] out IAvatar? avatar) => (avatar = GetAvatar(index)) is not null;

    public bool TryGetAvatarById<TAvatar>(Id id, [NotNullWhen(true)] out TAvatar? avatar) where TAvatar : IAvatar
    {
        return (avatar = GetAvatarById<TAvatar>(id)) is not null;
    }

    public bool TryGetAvatarByName<TAvatar>(string name, [NotNullWhen(true)] out TAvatar? avatar) where TAvatar : IAvatar
    {
        return (avatar = GetAvatar<TAvatar>(name)) is not null;
    }

    public bool TryGetAvatarByIndex<TAvatar>(int index, [NotNullWhen(true)] out TAvatar? avatar) where TAvatar : IAvatar
    {
        if (Avatars.TryGetValue(index, out Avatar? e))
        {
            if (e is TAvatar)
            {
                avatar = (TAvatar)(IAvatar)e;
                return true;
            }
        }

        avatar = default;
        return false;
    }
    #endregion
}
