![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Xabbo.Core?style=for-the-badge) ![Nuget](https://img.shields.io/nuget/dt/Xabbo.Core?style=for-the-badge)

# Xabbo.Core
A library for parsing data structures, managing the game state, and interacting with the server for Habbo Hotel.

### Building from source
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
