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
        var loader = new GameDataLoader() {
            CacheOnly = Environment.GetEnvironmentVariable("XABBO_TEST_GAMEDATA_FETCH") is null
        };
        var gameDataManager = new GameDataManager(loader);

        await gameDataManager.LoadAsync(Hotel.FromIdentifier(hotelIdentifier));
    }

    [ConditionalFact("XABBO_TEST_GAMEDATA")]
    public async Task TestFigureConverter()
    {
        var loader = new GameDataLoader() { CacheOnly = true };
        var modernManager = new GameDataManager(loader);
        var originsManager = new GameDataManager(loader);

        await Task.WhenAll(
            modernManager.LoadAsync(Hotel.FromIdentifier("us"), [GameDataType.FigureData]),
            originsManager.LoadAsync(Hotel.FromIdentifier("ous"), [GameDataType.FigureData])
        );

        var converter = new FigureConverter(modernManager.Figure!, originsManager.Figure!);
        var figure = converter.ToModern("2951027534180012550415002");

        _output.WriteLine(figure.ToString());
    }
}
