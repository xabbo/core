![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Xabbo.Core?style=for-the-badge) ![Nuget](https://img.shields.io/nuget/dt/Xabbo.Core?style=for-the-badge)

# xabbo/core
Provides data structures, parsers, composers, messages, game state, and game data management for
xabbo based extensions.

## Features

### Parsers and composers

Provides various parsers and composers for structures sent between the game client and server.

These work by implementing `IComposer` and `IParser<T>` from [xabbo/common](https://github.com/xabbo/common).

```cs
// Parsing an Avatar.
var avatar = packet.Read<Avatar>();
// You can now access avatar.Id, avatar.Name, avatar.Location, etc.

// Composing an Avatar.
packet.Write(avatar);

// Injecting a client-side user into the room.
int count = 1;
Id id = 1000; int index = 1000;
ext.Send(In.Users, count, new User(id, index)
{
    Name = "xabbo",
    Motto = "hello from xabbo/core",
    Location = (6, 6, 0),
    Figure = "hr-3090-42.hd-180-1.ch-3110-64-1408.lg-275-64.ha-1003-64"
});
```

### Messages

Various messages are provided which allow you to send structured messages to the client/server
without specifying the message name. These are also designed to be client-agnostic where possible,
meaning that they work on both the Flash and Shockwave clients, and are structured correctly
depending on the current session. These work by implementing `IMessage<T>` from xabbo/common.

```cs
// Sending messages: walking to a tile.
ext.Send(new WalkMsg(3, 4));
// The implementation writes 32-bit integers on Flash, and B64 encoded integers on Shockwave.
// This ensurest that the packet format is correct for the currently connected client.

// Intercepting outgoing chat messages.
ext.Intercept<ChatMsg>(chat => Console.WriteLine($"You said: {chat.Message}"));

// Intercepting and blocking incoming chat messages.
// By accepting 2 arguments you will have access to the Intercept instance,
// allowing you to block the intercepted packet.
ext.Intercept<AvatarChatMsg>((e, chat) => {
    if (chat.Message.Contains("block"))
        e.Block();
});

// Modifying messages: replacing "apple" with "orange" in incoming chat messages.
// By returning an IMessage, the packet will be replaced with that message.
ext.Intercept<AvatarChatMsg>(chat => chat with {
    Message = chat.Message.Replace("apple", "orange")
});
```

### Request messages

Request messages are a pair of request/response messages with a response data type.
This allows you to easily request data and receive its response asynchronously.
These work by implementing `IRequestMessage<TReq, TRes, TData>` from xabbo/common.

Let's take a look at the `GetUserDataMsg` which gets the user's own data.

It implements `IRequestMessage<GetUserDataMsg, UserDataMsg, UserData>`, which means it sends a
`GetUserDataMsg`, receives a `UserDataMsg` and returns a `UserData` object.

```cs
// In an async method...
Console.WriteLine("Requesting user data...");
// Sends a GetUserDataMsg, receives a UserDataMsg and returns a UserData object.
var userData = await ext.RequestAsync(new GetUserDataMsg());
// Now we have the UserData object which includes the user's name, ID, etc.
Console.WriteLine($"Received user data for: {userData.Name}");
```

### Game state managers

Game state managers such as the `RoomManager` intercept packets to track the state of the game. This
allows you to easily implement higher-level logic without needing to manually intercept and parse
packets.

```cs
// Log users who enter and leave the room, as well as chat messages.
var roomManager = new RoomManager(ext);
roomManager.AvatarsAdded += (e) => {
    if (e.Avatars.Length == 1)
        Console.WriteLine($"{e.Avatars[0].Name} entered the room.");
};
roomManager.AvatarRemoved += (e) => Console.WriteLine($"{e.Avatar.Name} left the room.");
roomManager.AvatarChat += (e) => Console.WriteLine($"{e.Avatar.Name}: {e.Message}");
```

### Game data management

A game data manager is provided which loads various resources such as the external texts, furni,
figure, and product data.

```cs
var gameDataManager = new GameDataManager();
gameDataManager.Loaded += () => Console.WriteLine($"Loaded {gameDataManager.Furni?.Count} furni");

// Load game data once a connection is established, as we will have which hotel to load game data for.
ext.Connected += (e) => {
    Task.Run(async () => {
        try
        {
            Console.WriteLine($"Loading game data for hotel: {e.Session.Hotel}.");
            await gameDataManager.LoadAsync(e.Session.Hotel);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load game data: {ex}");
        }
    });
};
```
outputs
```
Loading game data for hotel: US.
Loaded 15146 furni
```

### Core extensions

Various [extension methods](https://github.com/xabbo/core/blob/dev/src/Xabbo.Core/Extensions.cs) are
provided for convenience. For example, once game data has been loaded via the `GameDataManager`, you
can easily get the name of any item with the `GetName()` extension method. This works for any item
that implements the `IItem` interface, such as `Furni`, `InventoryItem`, `TradeItem`,
`MarketplaceItemInfo`, `CatalogProduct` etc.

```cs
// Log the name of all items in the room.
if (roomManager.EnsureRoom(out var room))
{
    foreach (var furni in room.Furni)
        Console.WriteLine($"#{furni.Id}: {furni.GetName()}");
}
```

## Building from source
Requires the [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

- Clone the repository & fetch submodules.
```
git clone https://github.com/xabbo/core xabbo/core
cd xabbo/core
git submodule update --init
git submodule foreach 'git checkout -b xabbo/core'
```
- Build with the .NET CLI.
```
dotnet build
```
