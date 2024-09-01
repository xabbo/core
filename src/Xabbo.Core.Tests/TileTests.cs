namespace Xabbo.Core.Tests;

public class TileTests
{
    [Theory]
    [InlineData(-0.002f, false)]
    [InlineData(-0.0016f, false)]
    [InlineData(-0.0015f, true)]
    [InlineData(-0.001f, true)]
    [InlineData(0, true)]
    [InlineData(0.001f, true)]
    [InlineData(0.0015f, true)]
    [InlineData(0.0016f, false)]
    [InlineData(0.002f, false)]
    public void TestDifferenceEquality(float diff, bool shouldEqual)
    {
        Tile a = (0, 0, 3.5f), b = (0, 0, 3.5f + diff);
        if (shouldEqual)
            Assert.Equal(a, b);
        else
            Assert.NotEqual(a, b);
    }

    [Fact]
    public void TestEqualityXY()
    {
        Tile
            a = (1, 2, 3),
            b = (1, 2, 4);
        Assert.True(a.XY == b);
        Assert.True(a == b.XY);
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
