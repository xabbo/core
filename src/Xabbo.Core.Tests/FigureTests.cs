namespace Xabbo.Core.Tests;

public class FigureTests
{
    [Theory]
    [InlineData("hd-180")]
    [InlineData("hd-180.hr-1")]
    [InlineData("hd-180.hr-1-2")]
    [InlineData("hd-180.hr-1-2-3.ch-4-5-6")]
    public void FigureEquality(string figureString)
    {
        Figure a = Figure.Parse(figureString);
        Figure b = Figure.Parse(figureString);

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
        Assert.Equal(a, b);
    }

    [Theory]
    [InlineData("")]
    [InlineData("hd")]
    [InlineData("hd-")]
    [InlineData("hd--1")]
    [InlineData("hd-180.hd-180")]
    [InlineData("hd-180..hr-1")]
    [InlineData("hd-180.-.hr-1")]
    [InlineData("hd-180. .hr-1")]
    [InlineData("hd-180.hr-")]
    public void InvalidFigures(string figureString)
    {
        Assert.ThrowsAny<Exception>(() => Figure.Parse(figureString));
    }

    [Fact]
    public void DuplicatePartTypes()
    {
        Assert.ThrowsAny<Exception>(() => {
            Figure figure = [];
            figure.Add(FigurePartType.Head, 1);
            figure.Add(FigurePartType.Head, 2);
        });
    }
}