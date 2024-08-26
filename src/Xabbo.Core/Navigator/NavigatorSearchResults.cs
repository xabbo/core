﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class NavigatorSearchResults : List<NavigatorSearchResultList>, IComposer, IParser<NavigatorSearchResults>
{
    public string Category { get; set; }
    public string Filter { get; set; }

    public NavigatorSearchResults(string category, string filter)
    {
        Category = category;
        Filter = filter;
    }

    private NavigatorSearchResults(in PacketReader p)
    {
        Category = p.Read<string>();
        Filter = p.Read<string>();
        int n = p.Read<Length>();
        for (int i = 0; i < n; i++)
            Add(p.Parse<NavigatorSearchResultList>());
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
        string? name = null,
        string? description = null,
        int? ownerId = null,
        string? owner = null,
        RoomAccess? access = null,
        TradePermissions? trading = null,
        RoomCategory? category = null,
        int? groupId = null,
        string? group = null)
    {
        foreach (var roomInfo in GetRooms())
        {
            if (name != null && !roomInfo.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase)) continue;
            if (description != null && !roomInfo.Description.Contains(description, StringComparison.CurrentCultureIgnoreCase)) continue;
            if (ownerId.HasValue && roomInfo.OwnerId != ownerId) continue;
            if (owner != null && !roomInfo.OwnerName.Equals(owner, StringComparison.InvariantCultureIgnoreCase)) continue;
            if (access.HasValue && roomInfo.Access != access) continue;
            if (trading.HasValue && roomInfo.Trading != trading) continue;
            if (category.HasValue && roomInfo.Category != category) continue;
            if (groupId.HasValue && (!roomInfo.IsGroupRoom || roomInfo.GroupId != groupId)) continue;
            if (group != null && (!roomInfo.IsGroupRoom || !roomInfo.GroupName.Contains(group, StringComparison.CurrentCultureIgnoreCase))) continue;
            yield return roomInfo;
        }
    }

    public RoomInfo? FindRoom(string name) => GetRooms().FirstOrDefault(
        roomInfo => roomInfo.Name.Contains(name, StringComparison.OrdinalIgnoreCase)
    );

    public void Compose(in PacketWriter p)
    {
        p.Write(Category);
        p.Write(Filter);
        p.Write<Length>(Count);
        for (int i = 0; i < Count; i++)
            p.Write(this[i]);
    }

    public static NavigatorSearchResults Parse(in PacketReader p) => new(in p);
}
