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

/// <inheritdoc cref="IAchievement"/>
public class Achievement : IAchievement, IParserComposer<Achievement>
{
    public int Id { get; set; }
    public int Level { get; set; }
    public string BadgeCode { get; set; } = "";
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

        Id = p.ReadInt();
        Level = p.ReadInt();
        BadgeCode = p.ReadString();
        BaseProgress = p.ReadInt();
        MaxProgress = p.ReadInt();
        LevelRewardPoints = p.ReadInt();
        LevelRewardPointType = p.ReadInt();
        CurrentProgress = p.ReadInt();
        IsComplete = p.ReadBool();
        Category = p.ReadString();
        Subcategory = p.ReadString();
        MaxLevel = p.ReadInt();
        DisplayMethod = p.ReadInt();
        State = p.ReadShort();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.WriteInt(Id);
        p.WriteInt(Level);
        p.WriteString(BadgeCode);
        p.WriteInt(BaseProgress);
        p.WriteInt(MaxProgress);
        p.WriteInt(LevelRewardPoints);
        p.WriteInt(LevelRewardPointType);
        p.WriteInt(CurrentProgress);
        p.WriteBool(IsComplete);
        p.WriteString(Category);
        p.WriteString(Subcategory);
        p.WriteInt(MaxLevel);
        p.WriteInt(DisplayMethod);
        p.WriteShort(State);
    }

    static Achievement IParser<Achievement>.Parse(in PacketReader p) => new(in p);
}
