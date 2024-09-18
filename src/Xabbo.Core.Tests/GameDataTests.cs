using Xabbo.Core.GameData;
using Xunit.Abstractions;

namespace Xabbo.Core.Tests;

public class GameDataTests(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [ConditionalTheory("XABBO_TEST_GAMEDATA")]
    [InlineData("us")]
    [InlineData("ous")]
    public async Task TestGameData(string hotelIdentifier)
    {
        var gameDataManager = new GameDataManager(".cache") {
            CacheOnly = Environment.GetEnvironmentVariable("XABBO_TEST_GAMEDATA_FETCH") is null
        };

        await gameDataManager.LoadAsync(Hotel.FromIdentifier(hotelIdentifier));
    }

    [ConditionalFact("XABBO_TEST_GAMEDATA")]
    public async Task TestFigureConverter()
    {
        var modernManager = new GameDataManager(".cache") { CacheOnly = true };
        var originsManager = new GameDataManager(".cache") { CacheOnly = true };

        await Task.WhenAll(
            modernManager.LoadAsync(Hotel.FromIdentifier("us"), [GameDataType.Figure]),
            originsManager.LoadAsync(Hotel.FromIdentifier("ous"), [GameDataType.Figure])
        );

        var converter = new FigureConverter(modernManager.Figure!, originsManager.Figure!);
        var figure = converter.ToModern("2951027534180012550415002");

        _output.WriteLine(figure.ToString());
    }
}
