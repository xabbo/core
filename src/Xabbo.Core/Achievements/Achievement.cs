using Xabbo.Messages;

namespace Xabbo.Core;

/*
    int achievementId
    int level
    string badgeId
    int scoreAtStartOfLevel
    int scoreLimit
      -> max(1, value) - scoreAtStartOfLevel
    int levelRewardPoints
    int levelRewardPointType
    int currentPoints
      -> value - scoreAtStartOfLevel
    bool finalLevel
    string category
    string subCategory
    int levelCount
    int displayMethod
    short state

    firstLevelAchieved = level > 1 || finalLevel
 */

public class Achievement : IAchievement, IComposer, IParser<Achievement>
{
    public int Id { get; set; }
    public int Level { get; set; }
    public string BadgeId { get; set; } = "";
    public int BaseProgress { get; set; }
    public int MaxProgress { get; set; }
    public int LevelRewardPoints { get; set; }
    public int LevelRewardPointType { get; set; }
    public int CurrentProgress { get; set; }
    public bool IsComplete { get; set; }
    public string Category { get; set; } = "";
    public string Subcategory { get; set; } = "";
    public int MaxLevel { get; set; }
    public int DisplayMethod { get; set; }
    public short State { get; set; }

    public Achievement() { }

    protected Achievement(in PacketReader p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        Id = p.Read<int>();
        Level = p.Read<int>();
        BadgeId = p.Read<string>();
        BaseProgress = p.Read<int>();
        MaxProgress = p.Read<int>();
        LevelRewardPoints = p.Read<int>();
        LevelRewardPointType = p.Read<int>();
        CurrentProgress = p.Read<int>();
        IsComplete = p.Read<bool>();
        Category = p.Read<string>();
        Subcategory = p.Read<string>();
        MaxLevel = p.Read<int>();
        DisplayMethod = p.Read<int>();
        State = p.Read<short>();
    }

    public void Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.Write(Id);
        p.Write(Level);
        p.Write(BadgeId);
        p.Write(BaseProgress);
        p.Write(MaxProgress);
        p.Write(LevelRewardPoints);
        p.Write(LevelRewardPointType);
        p.Write(CurrentProgress);
        p.Write(IsComplete);
        p.Write(Category);
        p.Write(Subcategory);
        p.Write(MaxLevel);
        p.Write(DisplayMethod);
        p.Write(State);
    }

    public static Achievement Parse(in PacketReader p) => new(in p);
}
