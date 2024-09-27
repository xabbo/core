using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IEmptyItemData"/>
public class EmptyItemData : ItemData, IEmptyItemData
{
    public EmptyItemData()
        : base(ItemDataType.Empty)
    { }

    public EmptyItemData(IEmptyItemData data)
        : this()
    { }

    protected override void Initialize(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);
    }

    protected override void WriteData(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);
    }
}
