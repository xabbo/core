using System;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a type of wired movement.
/// </summary>
public enum WiredMovementType
{
    /// <summary>
    /// Used when an avatar is moved by wired.
    /// </summary>
    Avatar = 0,
    /// <summary>
    /// Used when a floor item is moved by wired.
    /// </summary>
    FloorItem = 1,
    /// <summary>
    /// Used when a wall item is moved by wired.
    /// </summary>
    WallItem = 2,
    /// <summary>
    /// Used when an avatar's direction is updated by wired.
    /// </summary>
    AvatarDirection = 3
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
            WiredMovementType.Avatar => new AvatarWiredMovement(in p),
            WiredMovementType.FloorItem => new FloorItemWiredMovement(in p),
            WiredMovementType.WallItem => new WallItemWiredMovement(in p),
            WiredMovementType.AvatarDirection => new AvatarDirectionWiredMovement(in p),
            _ => throw new Exception($"Unknown wired movement type: {type}."),
        };
    }
}

/// <summary>
/// Defines the parameters of an avatar being moved by wired.
/// </summary>
public class AvatarWiredMovement : WiredMovement
{
    public Tile Source { get; set; }
    public Tile Destination { get; set; }
    public int AvatarIndex { get; set; }
    public bool IsSlide { get; set; }
    public int BodyDirection { get; set; }
    public int HeadDirection { get; set; }

    public AvatarWiredMovement() : base(WiredMovementType.Avatar) { }

    internal AvatarWiredMovement(in PacketReader p) : this()
    {
        int srcX = p.ReadInt();
        int srcY = p.ReadInt();
        int dstX = p.ReadInt();
        int dstY = p.ReadInt();
        float srcZ = p.ReadFloat();
        float dstZ = p.ReadFloat();
        Source = new Tile(srcX, srcY, srcZ);
        Destination = new Tile(dstX, dstY, dstZ);
        AvatarIndex = p.ReadInt();
        IsSlide = p.ReadInt() != 0;
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
        p.WriteInt(AvatarIndex);
        p.WriteInt(IsSlide ? 1 : 0);
        p.WriteInt(AnimationTime);
        p.WriteInt(BodyDirection);
        p.WriteInt(HeadDirection);
    }
}

/// <summary>
/// Defines the parameters of a floor item being moved by wired.
/// </summary>
public class FloorItemWiredMovement : WiredMovement
{
    public Tile Source { get; set; }
    public Tile Destination { get; set; }
    public Id ItemId { get; set; }
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
        ItemId = p.ReadId();
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
        p.WriteId(ItemId);
        p.WriteInt(AnimationTime);
        p.WriteInt(Rotation);
    }
}

/// <summary>
/// Defines the parameters of a wall item being moved by wired.
/// </summary>
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

/// <summary>
/// Defines the parameters of an avatar's direction being changed by wired.
/// </summary>
public class AvatarDirectionWiredMovement : WiredMovement
{
    public int AvatarIndex { get; set; }
    public int BodyDirection { get; set; }
    public int HeadDirection { get; set; }

    public AvatarDirectionWiredMovement() : base(WiredMovementType.WallItem) { }

    internal AvatarDirectionWiredMovement(in PacketReader p) : this()
    {
        AvatarIndex = p.ReadInt();
        BodyDirection = p.ReadInt();
        HeadDirection = p.ReadInt();
    }

    protected override void Compose(in PacketWriter p)
    {
        base.Compose(p);
        p.WriteInt(AvatarIndex);
        p.WriteInt(BodyDirection);
        p.WriteInt(HeadDirection);
    }
}
