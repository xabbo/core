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
    WallItem = 2
}

/// <summary>
/// Defines the base parameters of a wired movement.
/// </summary>
public abstract class WiredMovement : IComposer, IParser<WiredMovement>
{
    public WiredMovementType Type { get; }
    public int AnimationTime { get; set; }

    protected WiredMovement(WiredMovementType type)
    {
        Type = type;
    }

    public virtual void Compose(in PacketWriter p)
    {
        p.Write((int)Type);
    }

    public static WiredMovement Parse(in PacketReader p)
    {
        var type = (WiredMovementType)p.Read<int>();
        return type switch
        {
            WiredMovementType.User => new UserWiredMovement(in p),
            WiredMovementType.FloorItem => new FloorItemWiredMovement(in p),
            WiredMovementType.WallItem => new WallItemWiredMovement(in p),
            _ => throw new Exception($"Unknown wired movement type: {type}"),
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
        int srcX = p.Read<int>();
        int srcY = p.Read<int>();
        int dstX = p.Read<int>();
        int dstY = p.Read<int>();
        float srcZ = p.Read<float>();
        float dstZ = p.Read<float>();
        Source = new Tile(srcX, srcY, srcZ);
        Destination = new Tile(dstX, dstY, dstZ);
        UserIndex = p.Read<int>();
        Slide = p.Read<int>() != 0;
        AnimationTime = p.Read<int>();
        BodyDirection = p.Read<int>();
        HeadDirection = p.Read<int>();
    }

    public override void Compose(in PacketWriter p)
    {
        base.Compose(in p);
        p.Write(Source.X);
        p.Write(Source.Y);
        p.Write(Destination.X);
        p.Write(Destination.Y);
        p.Write(Source.Z);
        p.Write(Destination.Z);
        p.Write(UserIndex);
        p.Write(Slide ? 1 : 0);
        p.Write(AnimationTime);
        p.Write(BodyDirection);
        p.Write(HeadDirection);
    }
}

public class FloorItemWiredMovement : WiredMovement
{
    public Tile Source { get; set; }
    public Tile Destination { get; set; }
    public long FurniId { get; set; }
    public int Rotation { get; set; }

    public FloorItemWiredMovement() : base(WiredMovementType.FloorItem) { }

    internal FloorItemWiredMovement(in PacketReader p) : this()
    {
        int srcX = p.Read<int>();
        int srcY = p.Read<int>();
        int dstX = p.Read<int>();
        int dstY = p.Read<int>();
        float srcZ = p.Read<float>();
        float dstZ = p.Read<float>();
        Source = new Tile(srcX, srcY, srcZ);
        Destination = new Tile(dstX, dstY, dstZ);
        FurniId = p.Read<Id>();
        AnimationTime = p.Read<int>();
        Rotation = p.Read<int>();
    }

    public override void Compose(in PacketWriter p)
    {
        base.Compose(in p);
        p.Write(Source.X);
        p.Write(Source.Y);
        p.Write(Destination.X);
        p.Write(Destination.Y);
        p.Write(Source.Z);
        p.Write(Destination.Z);
        p.Write(FurniId);
        p.Write(AnimationTime);
        p.Write(Rotation);
    }
}

public class WallItemWiredMovement : WiredMovement
{
    public long ItemId { get; set; }
    public WallLocation Source { get; set; }
    public WallLocation Destination { get; set; }

    public WallItemWiredMovement() : base(WiredMovementType.WallItem) { }

    internal WallItemWiredMovement(in PacketReader p) : this()
    {
        ItemId = p.Read<Id>();
        var orientation = p.Read<bool>() ? WallOrientation.Right : WallOrientation.Left;
        int srcWX = p.Read<int>();
        int srcWY = p.Read<int>();
        int srcLX = p.Read<int>();
        int srcLY = p.Read<int>();
        int dstWX = p.Read<int>();
        int dstWY = p.Read<int>();
        int dstLX = p.Read<int>();
        int dstLY = p.Read<int>();
        Source = new WallLocation(srcWX, srcWY, srcLX, srcLY, orientation);
        Destination = new WallLocation(dstWX, dstWY, dstLX, dstLY, orientation);
        AnimationTime = p.Read<int>();
    }

    public override void Compose(in PacketWriter p)
    {
        base.Compose(p);
        p.Write(ItemId);
        p.Write(Destination.Orientation.IsRight);
        p.Write(Source.WX);
        p.Write(Source.WY);
        p.Write(Source.LX);
        p.Write(Source.LY);
        p.Write(Destination.WX);
        p.Write(Destination.WY);
        p.Write(Destination.LX);
        p.Write(Destination.LY);
        p.Write(AnimationTime);
    }
}