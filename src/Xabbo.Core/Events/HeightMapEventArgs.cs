namespace Xabbo.Core.Events;

public sealed class HeightMapEventArgs(IHeightmap heightmap)
{
    public IHeightmap Heightmap { get; } = heightmap;
}
