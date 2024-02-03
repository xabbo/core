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
public abstract class WiredMovement : IComposable
{
    public WiredMovementType Type { get; }
    public int AnimationTime { get; set; }

    protected WiredMovement(WiredMovementType type)
    {
        Type = type;
    }

    public virtual void Compose(IPacket packet)
    {
        packet.WriteInt((int)Type);
    }

    public static WiredMovement Parse(IReadOnlyPacket packet)
    {
        var type = (WiredMovementType)packet.ReadInt();
        return type switch
        {
            WiredMovementType.User => new UserWiredMovement(packet),
            WiredMovementType.FloorItem => new FloorItemWiredMovement(packet),
            WiredMovementType.WallItem => new WallItemWiredMovement(packet),
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

    internal UserWiredMovement(IReadOnlyPacket packet) : this()
    {
        int srcX = packet.ReadInt();
        int srcY = packet.ReadInt();
        int dstX = packet.ReadInt();
        int dstY = packet.ReadInt();
        float srcZ = packet.ReadFloatAsString();
        float dstZ = packet.ReadFloatAsString();
        Source = new Tile(srcX, srcY, srcZ);
        Destination = new Tile(dstX, dstY, dstZ);
        UserIndex = packet.ReadInt();
        Slide = packet.ReadInt() != 0;
        AnimationTime = packet.ReadInt();
        BodyDirection = packet.ReadInt();
        HeadDirection = packet.ReadInt();
    }

    public override void Compose(IPacket packet)
    {
        base.Compose(packet);
        packet
            .WriteInt(Source.X)
            .WriteInt(Source.Y)
            .WriteInt(Destination.X)
            .WriteInt(Destination.Y)
            .WriteFloatAsString(Source.Z)
            .WriteFloatAsString(Destination.Z)
            .WriteInt(UserIndex)
            .WriteInt(Slide ? 1 : 0)
            .WriteInt(AnimationTime)
            .WriteInt(BodyDirection)
            .WriteInt(HeadDirection);
    }
}

public class FloorItemWiredMovement : WiredMovement
{
    public Tile Source { get; set; }
    public Tile Destination { get; set; }
    public long FurniId { get; set; }
    public int Rotation { get; set; }

    public FloorItemWiredMovement() : base(WiredMovementType.FloorItem) { }

    internal FloorItemWiredMovement(IReadOnlyPacket packet) : this()
    {
        int srcX = packet.ReadInt();
        int srcY = packet.ReadInt();
        int dstX = packet.ReadInt();
        int dstY = packet.ReadInt();
        float srcZ = packet.ReadFloatAsString();
        float dstZ = packet.ReadFloatAsString();
        Source = new Tile(srcX, srcY, srcZ);
        Destination = new Tile(dstX, dstY, dstZ);
        FurniId = packet.ReadLegacyLong();
        AnimationTime = packet.ReadInt();
        Rotation = packet.ReadInt();
    }

    public override void Compose(IPacket packet)
    {
        base.Compose(packet);
        packet
            .WriteInt(Source.X)
            .WriteInt(Source.Y)
            .WriteInt(Destination.X)
            .WriteInt(Destination.Y)
            .WriteFloatAsString(Source.Z)
            .WriteFloatAsString(Destination.Z)
            .WriteLegacyLong(FurniId)
            .WriteInt(AnimationTime)
            .WriteInt(Rotation);
    }
}

public class WallItemWiredMovement : WiredMovement
{
    public long ItemId { get; set; }
    public WallLocation Source { get; set; }
    public WallLocation Destination { get; set; }

    public WallItemWiredMovement() : base(WiredMovementType.WallItem) { }

    internal WallItemWiredMovement(IReadOnlyPacket packet) : this()
    {
        ItemId = packet.ReadLegacyLong();
        var orientation = packet.ReadBool() ? WallOrientation.Right : WallOrientation.Left;
        int srcWX = packet.ReadInt();
        int srcWY = packet.ReadInt();
        int srcLX = packet.ReadInt();
        int srcLY = packet.ReadInt();
        int dstWX = packet.ReadInt();
        int dstWY = packet.ReadInt();
        int dstLX = packet.ReadInt();
        int dstLY = packet.ReadInt();
        Source = new WallLocation(srcWX, srcWY, srcLX, srcLY, orientation);
        Destination = new WallLocation(dstWX, dstWY, dstLX, dstLY, orientation);
        AnimationTime = packet.ReadInt();
    }

    public override void Compose(IPacket packet)
    {
        base.Compose(packet);
        packet
            .WriteLegacyLong(ItemId)
            .WriteBool(Destination.Orientation.IsRight)
            .WriteInt(Source.WX)
            .WriteInt(Source.WY)
            .WriteInt(Source.LX)
            .WriteInt(Source.LY)
            .WriteInt(Destination.WX)
            .WriteInt(Destination.WY)
            .WriteInt(Destination.LX)
            .WriteInt(Destination.LY)
            .WriteInt(AnimationTime);
    }
}