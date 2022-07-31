using System;

namespace Xabbo.Core.Tests;

public class TileTests
{
    [Fact(DisplayName = "Tile == Tuple")]
    public void Tile_Equals_Tuple()
    {
        Tile tile = new(1, 2, 3.4f);

        // Compare X,Y,Z
        Assert.True(tile == (1, 2, 3.4f));

        // Compare X,Y only
        Assert.True(tile == (1, 2));

        // Compare Z with epsilon check
        Assert.True(tile == (1, 2, 3.4001f));
        Assert.True(tile == (1, 2, 3.401f));
        Assert.False(tile == (1, 2, 3.41f));
    }
}
