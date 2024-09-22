using System;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a type of wired movement.
/// </summary>
public enum WiredMovementType
{
    /// <summary>
    /// Used when a user is moved by wired.
    /// </summary>
    User = 0,
    /// <summary>
    /// Used when a floor item is moved by wired.
    /// </summary>
    FloorItem = 1,
    /// <summary>
    /// Used when a wall item is moved by wired.
    /// </summary>
    WallItem = 2,
    /// <summary>
    /// Used when a user's direction is updated by wired.
    /// </summary>
    UserDirection
}

/// <summary>
/// Defines the base parameters of a wired movement.
/// </summary>
public abstract class WiredMovement(WiredMovementType type) : IParserComposer<WiredMovement>
{
    public WiredMovementType Type { get; } = type;
    public int AnimationTime { get; set; }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);
        Compose(in p);
    }
    protected virtual void Compose(in PacketWriter p) => p.WriteInt((int)Type);

    static WiredMovement IParser<WiredMovement>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);

        var type = (WiredMovementType)p.ReadInt();
        return type switch
        {
            WiredMovementType.User => new UserWiredMovement(in p),
            WiredMovementType.FloorItem => new FloorItemWiredMovement(in p),
            WiredMovementType.WallItem => new WallItemWiredMovement(in p),
            WiredMovementType.UserDirection => new UserDirectionWiredMovement(in p),
            _ => throw new Exception($"Unknown wired movement type: {type}."),
        };
    }
}

/// <summary>
/// Defines the parameters of a user wired movement.
/// </summary>
public class UserWiredMovement : WiredMovement
{
    public Tile Source { get; set; }
    public Tile Destination { get; set; }
    public int UserIndex { get; set; }
    public bool Slide { get; set; }
    public int BodyDirection { get; set; }
    public int HeadDirection { get; set; }

    public UserWiredMovement() : base(WiredMovementType.User) { }

    internal UserWiredMovement(in PacketReader p) : this()
    {
        int srcX = p.ReadInt();
        int srcY = p.ReadInt();
        int dstX = p.ReadInt();
        int dstY = p.ReadInt();
        float srcZ = p.ReadFloat();
        float dstZ = p.ReadFloat();
        Source = new Tile(srcX, srcY, srcZ);
        Destination = new Tile(dstX, dstY, dstZ);
        UserIndex = p.ReadInt();
        Slide = p.ReadInt() != 0;
        AnimationTime = p.ReadInt();
        BodyDirection = p.ReadInt();
        HeadDirection = p.ReadInt();
    }

    protected override void Compose(in PacketWriter p)
    {
        base.Compose(in p);
        p.WriteInt(Source.X);
        p.WriteInt(Source.Y);
        p.WriteInt(Destination.X);
        p.WriteInt(Destination.Y);
        p.WriteFloat(Source.Z);
        p.WriteFloat(Destination.Z);
        p.WriteInt(UserIndex);
        p.WriteInt(Slide ? 1 : 0);
        p.WriteInt(AnimationTime);
        p.WriteInt(BodyDirection);
        p.WriteInt(HeadDirection);
    }
}

public class FloorItemWiredMovement : WiredMovement
{
    public Tile Source { get; set; }
    public Tile Destination { get; set; }
    public Id FurniId { get; set; }
    public int Rotation { get; set; }

    public FloorItemWiredMovement() : base(WiredMovementType.FloorItem) { }

    internal FloorItemWiredMovement(in PacketReader p) : this()
    {
        int srcX = p.ReadInt();
        int srcY = p.ReadInt();
        int dstX = p.ReadInt();
        int dstY = p.ReadInt();
        float srcZ = p.ReadFloat();
        float dstZ = p.ReadFloat();
        Source = new Tile(srcX, srcY, srcZ);
        Destination = new Tile(dstX, dstY, dstZ);
        FurniId = p.ReadId();
        AnimationTime = p.ReadInt();
        Rotation = p.ReadInt();
    }

    protected override void Compose(in PacketWriter p)
    {
        base.Compose(in p);
        p.WriteInt(Source.X);
        p.WriteInt(Source.Y);
        p.WriteInt(Destination.X);
        p.WriteInt(Destination.Y);
        p.WriteFloat(Source.Z);
        p.WriteFloat(Destination.Z);
        p.WriteId(FurniId);
        p.WriteInt(AnimationTime);
        p.WriteInt(Rotation);
    }
}

public class WallItemWiredMovement : WiredMovement
{
    public Id ItemId { get; set; }
    public WallLocation Source { get; set; }
    public WallLocation Destination { get; set; }

    public WallItemWiredMovement() : base(WiredMovementType.WallItem) { }

    internal WallItemWiredMovement(in PacketReader p) : this()
    {
        ItemId = p.ReadId();
        var orientation = p.ReadBool() ? WallOrientation.Right : WallOrientation.Left;
        int srcWX = p.ReadInt();
        int srcWY = p.ReadInt();
        int srcLX = p.ReadInt();
        int srcLY = p.ReadInt();
        int dstWX = p.ReadInt();
        int dstWY = p.ReadInt();
        int dstLX = p.ReadInt();
        int dstLY = p.ReadInt();
        Source = new WallLocation(srcWX, srcWY, srcLX, srcLY, orientation);
        Destination = new WallLocation(dstWX, dstWY, dstLX, dstLY, orientation);
        AnimationTime = p.ReadInt();
    }

    protected override void Compose(in PacketWriter p)
    {
        base.Compose(p);
        p.WriteId(ItemId);
        p.WriteBool(Destination.Orientation.IsRight);
        p.WriteInt(Source.Wall.X);
        p.WriteInt(Source.Wall.Y);
        p.WriteInt(Source.Offset.X);
        p.WriteInt(Source.Offset.Y);
        p.WriteInt(Destination.Wall.X);
        p.WriteInt(Destination.Wall.Y);
        p.WriteInt(Destination.Offset.X);
        p.WriteInt(Destination.Offset.Y);
        p.WriteInt(AnimationTime);
    }
}

public class UserDirectionWiredMovement : WiredMovement
{
    public int Index { get; set; }
    public int BodyDirection { get; set; }
    public int HeadDirection { get; set; }

    public UserDirectionWiredMovement() : base(WiredMovementType.WallItem) { }

    internal UserDirectionWiredMovement(in PacketReader p) : this()
    {
        Index = p.ReadInt();
        BodyDirection = p.ReadInt();
        HeadDirection = p.ReadInt();
    }

    protected override void Compose(in PacketWriter p)
    {
        base.Compose(p);
        p.WriteInt(Index);
        p.WriteInt(BodyDirection);
        p.WriteInt(HeadDirection);
    }
}