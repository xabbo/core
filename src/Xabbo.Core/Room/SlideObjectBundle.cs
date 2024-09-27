using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a group of objects being moved by a roller.
/// </summary>
/// <remarks>
/// Associates a single roller with a group of floor item slide movements
/// and/or a single avatar slide movement.
/// </remarks>
public sealed class SlideObjectBundle : IParserComposer<SlideObjectBundle>
{
    /// <summary>
    /// The position the objects are sliding from.
    /// </summary>
    public Point From { get; set; }

    /// <summary>
    /// The position the objects are sliding to.
    /// </summary>
    public Point To { get; set; }

    /// <summary>
    /// The group of objects being moved by the roller.
    /// </summary>
    public List<SlideObject> SlideObjects { get; set; }

    /// <summary>
    /// The ID of the roller that caused the slide.
    /// </summary>
    public Id RollerId { get; set; }

    /// <summary>
    /// The type of the avatar slide, if in avatar is being moved.
    /// </summary>
    public AvatarSlideType AvatarSlideType { get; set; }

    /// <summary>
    /// The avatar being moved by the roller.
    /// </summary>
    /// <remarks>
    /// Available if <see cref="AvatarSlideType"/> is
    /// <see cref="AvatarSlideType.WalkingAvatar"/>
    /// or <see cref="AvatarSlideType.StandingAvatar"/>.
    /// </remarks>
    public SlideAvatar? Avatar { get; set; }

    /// <summary>
    /// Constructs a new empty <see cref="SlideObjectBundle"/>.
    /// </summary>
    public SlideObjectBundle()
    {
        SlideObjects = [];
        AvatarSlideType = AvatarSlideType.None;
    }

    private SlideObjectBundle(in PacketReader p)
    {
        From = p.Parse<Point>();
        To = p.Parse<Point>();

        SlideObjects = [.. p.ParseArray<SlideObject>()];

        RollerId = p.ReadId();

        AvatarSlideType = AvatarSlideType.None;
        if (p.Available > 0)
        {
            AvatarSlideType = (AvatarSlideType)p.ReadInt();
            if (AvatarSlideType == AvatarSlideType.WalkingAvatar ||
                AvatarSlideType == AvatarSlideType.StandingAvatar)
            {
                if (p.Client is ClientType.Unity)
                    p.ReadInt(); // ?
                Avatar = p.Parse<SlideAvatar>();
            }
        }
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.Compose(From);
        p.Compose(To);

        p.ComposeArray(SlideObjects);

        p.WriteId(RollerId);

        if (AvatarSlideType is not AvatarSlideType.None)
        {
            p.WriteInt((int)AvatarSlideType);
            if (Avatar is not null &&
                AvatarSlideType is AvatarSlideType.WalkingAvatar or AvatarSlideType.StandingAvatar)
            {
                p.Compose(Avatar);
            }
        }
    }

    static SlideObjectBundle IParser<SlideObjectBundle>.Parse(in PacketReader p) => new(in p);
}
