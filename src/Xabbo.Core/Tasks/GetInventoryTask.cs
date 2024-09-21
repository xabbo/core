using System;

using Xabbo.Messages.Flash;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

using Xabbo.Core.Game;

namespace Xabbo.Core.Tasks;

[Intercept(~ClientType.Shockwave)]
public sealed partial class GetInventoryTask(IInterceptor interceptor, bool blockPackets = true) : InterceptorTask<IInventory>(interceptor)
{
    private int _total = -1, _index = 0;
    private readonly Inventory inventory = new();

    private readonly bool _blockPackets = blockPackets;

    protected override ClientType SupportedClients => ~ClientType.Shockwave;

    public GetInventoryTask(IInterceptor interceptor) : this(interceptor, true) { }

    protected override void OnExecute() => Interceptor.Send(Out.RequestFurniInventory);

    [InterceptIn(nameof(In.FurniList))]
    void OnInventoryItems(Intercept e)
    {
        try
        {
            InventoryFragment fragment = e.Packet.Read<InventoryFragment>();

            if (fragment.Index != _index)
            {
                throw new Exception(
                    $"Fragment index mismatch."
                    + $" Expected: {_index}; received: {fragment.Index}."
                );
            }

            if (_total == -1)
            {
                _total = fragment.Total;
            }
            else if (fragment.Total != _total)
            {
                throw new Exception(
                    "Fragment count mismatch."
                    + $" Expected: {_total}; received: {fragment.Total}."
                );
            }

            _index++;

            if (_blockPackets)
                e.Block();

            foreach (var item in fragment)
                inventory.TryAdd(item);

            if (_index == _total)
                SetResult(inventory);
        }
        catch (Exception ex) { SetException(ex); }
    }
}
