using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class SlideObjectBundle : IParserComposer<SlideObjectBundle>
{
    public Point From { get; set; }
    public Point To { get; set; }
    public List<SlideObject> SlideObjects { get; set; }
    public Id RollerId { get; set; }
    public SlideType Type { get; set; }
    public SlideEntity? Entity { get; set; }

    public SlideObjectBundle()
    {
        SlideObjects = [];
        Type = SlideType.None;
    }

    private SlideObjectBundle(in PacketReader p)
    {
        From = p.Parse<Point>();
        To = p.Parse<Point>();

        SlideObjects = [.. p.ParseArray<SlideObject>()];

        RollerId = p.ReadId();

        Type = SlideType.None;
        if (p.Available > 0)
        {
            Type = (SlideType)p.ReadInt();
            if (Type == SlideType.WalkingEntity ||
                Type == SlideType.StandingEntity)
            {
                if (p.Client is ClientType.Unity)
                    p.ReadInt(); // ?
                Entity = p.Parse<SlideEntity>();
            }
        }
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.Compose(From);
        p.Compose(To);

        p.ComposeArray(SlideObjects);

        p.WriteId(RollerId);

        if (Type is not SlideType.None)
        {
            p.WriteInt((int)Type);
            if (Entity is not null &&
                Type is SlideType.WalkingEntity or SlideType.StandingEntity)
            {
                p.Compose(Entity);
            }
        }
    }

    static SlideObjectBundle IParser<SlideObjectBundle>.Parse(in PacketReader p) => new(in p);
}
