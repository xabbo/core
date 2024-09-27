using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IUser"/>
public class User(Id id, int index) : Avatar(AvatarType.User, id, index), IUser
{
    public Gender Gender { get; set; } = Gender.Unisex;
    public Id GroupId { get; set; } = -1;
    public int GroupStatus { get; set; }
    public string GroupName { get; set; } = "";
    public string FigureExtra { get; set; } = "";
    public int AchievementScore { get; set; }
    public bool IsStaff { get; set; }
    public string BadgeCode { get; set; } = "";

    public RightsLevel RightsLevel => CurrentUpdate?.RightsLevel ?? RightsLevel.None;
    public bool HasRights => RightsLevel > 0;

    internal User(Id id, int index, in PacketReader p)
        : this(id, index)
    {
        if (p.Client is ClientType.Shockwave)
            return;

        Gender = H.ToGender(p.ReadString());
        GroupId = p.ReadId();
        GroupStatus = p.ReadInt();
        GroupName = p.ReadString();
        FigureExtra = p.ReadString();
        AchievementScore = p.ReadInt();
        IsStaff = p.ReadBool();
    }

    protected override void OnUpdate(AvatarStatus update) { }

    public override void Compose(in PacketWriter p)
    {
        base.Compose(in p);

        if (p.Client is ClientType.Shockwave)
            return;

        p.WriteString(Gender.ToClientString().ToLower());
        p.WriteId(GroupId);
        p.WriteInt(GroupStatus);
        p.WriteString(GroupName);
        p.WriteString(FigureExtra);
        p.WriteInt(AchievementScore);
        p.WriteBool(IsStaff);
    }
}
