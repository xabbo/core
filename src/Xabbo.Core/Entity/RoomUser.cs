using Xabbo.Messages;

namespace Xabbo.Core;

public class RoomUser : Entity, IRoomUser
{
    public Gender Gender { get; set; }
    public long GroupId { get; set; }
    public int GroupStatus { get; set; }
    public string GroupName { get; set; }
    public string FigureExtra { get; set; }
    public int AchievementScore { get; set; }
    public bool IsModerator { get; set; }

    public int RightsLevel => CurrentUpdate?.ControlLevel ?? 0;
    public bool HasRights => RightsLevel > 0;

    public RoomUser(long id, int index)
        : base(EntityType.User, id, index)
    {
        Gender = Gender.Unisex;
        GroupId = -1;
        GroupName = "";
        FigureExtra = "";
    }

    internal RoomUser(long id, int index, in PacketReader p)
        : this(id, index)
    {
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

        p.Write(Gender.ToShortString().ToLower());
        p.Write(GroupId);
        p.Write(GroupStatus);
        p.Write(GroupName);
        p.Write(FigureExtra);
        p.Write(AchievementScore);
        p.Write(IsModerator);
    }
}
