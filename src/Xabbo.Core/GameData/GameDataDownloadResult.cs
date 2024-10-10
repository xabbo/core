namespace Xabbo.Core.GameData;

public sealed record GameDataDownloadResult(
    GameDataType Type,
    string Hash,
    string FilePath
);