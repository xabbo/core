using Xabbo.Messages;

namespace Xabbo.Core;

public class RoomUser(long id, int index) : Entity(EntityType.User, id, index), IRoomUser
{
    public Gender Gender { get; set; } = Gender.Unisex;
    public long GroupId { get; set; } = -1;
    public int GroupStatus { get; set; }
    public string GroupName { get; set; } = "";
    public string FigureExtra { get; set; } = "";
    public int AchievementScore { get; set; }
    public bool IsModerator { get; set; }
    public string BadgeCode { get; set; } = "";

    public int RightsLevel => CurrentUpdate?.ControlLevel ?? 0;
    public bool HasRights => RightsLevel > 0;

    internal RoomUser(long id, int index, in PacketReader p)
        : this(id, index)
    {
        if (p.Client == ClientType.Shockwave)
            return;

        Gender = H.ToGender(p.Read<string>());
        GroupId = p.Read<Id>();
        GroupStatus = p.Read<int>();
        GroupName = p.Read<string>();
        FigureExtra = p.Read<string>();
        AchievementScore = p.Read<int>();
        IsModerator = p.Read<bool>();
    }

    protected override void OnUpdate(EntityStatusUpdate update) { }

    public override void Compose(in PacketWriter p)
    {
        base.Compose(in p);

        if (p.Client == ClientType.Shockwave)
            return;

        p.Write(Gender.ToShortString().ToLower());
        p.Write(GroupId);
        p.Write(GroupStatus);
        p.Write(GroupName);
        p.Write(FigureExtra);
        p.Write(AchievementScore);
        p.Write(IsModerator);
    }
}
