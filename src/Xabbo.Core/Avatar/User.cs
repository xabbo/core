using Xabbo.Messages;

namespace Xabbo.Core;

public class User(Id id, int index) : Avatar(AvatarType.User, id, index), IUser
{
    public Gender Gender { get; set; } = Gender.Unisex;
    public Id GroupId { get; set; } = -1;
    public int GroupStatus { get; set; }
    public string GroupName { get; set; } = "";
    public string FigureExtra { get; set; } = "";
    public int AchievementScore { get; set; }
    public bool IsModerator { get; set; }
    public string BadgeCode { get; set; } = "";

    public int RightsLevel => CurrentUpdate?.ControlLevel ?? 0;
    public bool HasRights => RightsLevel > 0;

    internal User(long id, int index, in PacketReader p)
        : this(id, index)
    {
        if (p.Client == ClientType.Shockwave)
            return;

        Gender = H.ToGender(p.ReadString());
        GroupId = p.ReadId();
        GroupStatus = p.ReadInt();
        GroupName = p.ReadString();
        FigureExtra = p.ReadString();
        AchievementScore = p.ReadInt();
        IsModerator = p.ReadBool();
    }

    protected override void OnUpdate(AvatarStatusUpdate update) { }

    public override void Compose(in PacketWriter p)
    {
        base.Compose(in p);

        if (p.Client == ClientType.Shockwave)
            return;

        p.WriteString(Gender.ToShortString().ToLower());
        p.WriteId(GroupId);
        p.WriteInt(GroupStatus);
        p.WriteString(GroupName);
        p.WriteString(FigureExtra);
        p.WriteInt(AchievementScore);
        p.WriteBool(IsModerator);
    }
}
