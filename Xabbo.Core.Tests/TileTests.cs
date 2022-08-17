using System;

namespace Xabbo.Core.Tests;

public class TileTests
{
    [Fact]
    public void Equality()
    {
        Tile tile = (1, 2, 3.4f);

        // Compare X,Y,Z
        Assert.True(tile == (1, 2, 3.4f));
        Assert.False(tile != (1, 2, 3.4f));

        Assert.True((1, 2, 3.4f) == tile);
        Assert.False((1, 2, 3.4f) != tile);

        // Compare X,Y only
        Assert.True(tile == (1, 2));
        Assert.False(tile != (1, 2));

        Assert.True((1, 2) == tile);
        Assert.False((1, 2) != tile);

        // Compare Z with epsilon check
        Assert.True(tile == (1, 2, 3.4001f));
        Assert.False(tile != (1, 2, 3.4001f));

        Assert.True(tile == (1, 2, 3.401f));
        Assert.False(tile != (1, 2, 3.401f));

        Assert.False(tile == (1, 2, 3.41f));
        Assert.True(tile != (1, 2, 3.41f));
    }

    [Fact]
    public void Addition()
    {
        Tile
            a = (1, 2, 3),
            b = (4, 5, 6),
            c = a + b;

        Assert.Equal(a.X + b.X, c.X);
        Assert.Equal(a.Y + b.Y, c.Y);
        Assert.Equal(a.Z + b.Z, c.Z);
    }

    [Fact]
    public void Addition_by_Point()
    {
        Tile
            a = (1, 2, 3),
            b = (4, 5, 6),
            c = a + b.XY;

        Assert.Equal(a.X + b.X, c.X);
        Assert.Equal(a.Y + b.Y, c.Y);
        Assert.Equal(a.Z, c.Z);
    }

    [Fact]
    public void Subtraction()
    {
        Tile
            a = (1, 2, 3),
            b = (4, 5, 6),
            c = a - b;

        Assert.Equal(a.X - b.X, c.X);
        Assert.Equal(a.Y - b.Y, c.Y);
        Assert.Equal(a.Z - b.Z, c.Z);
    }

    [Fact]
    public void Subtraction_by_Point()
    {
        Tile
            a = (1, 2, 3),
            b = (4, 5, 6),
            c = a - b.XY;

        Assert.Equal(a.X - b.X, c.X);
        Assert.Equal(a.Y - b.Y, c.Y);
        Assert.Equal(a.Z, c.Z);
    }
}
