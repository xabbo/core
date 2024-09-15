using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xabbo.Extension;
using Xabbo.Messages.Flash;

using Xabbo.Core.Events;

namespace Xabbo.Core.Game;

/// <summary>
/// Manages the user's inventory.
/// </summary>
[Intercept(~ClientType.Shockwave)]
public sealed partial class InventoryManager(IExtension extension, ILoggerFactory? loggerFactory = null) : GameStateManager(extension)
{
    private readonly ILogger Log = (ILogger?)loggerFactory?.CreateLogger<InventoryManager>() ?? NullLogger.Instance;

    private readonly List<InventoryFragment> _fragments = [];
    private bool _forceLoadingInventory;
    private int _currentPacketIndex;
    private int _totalPackets;

    private TaskCompletionSource<IInventory> _loadTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

    private Inventory? _inventory;
    public IInventory? Inventory => _inventory;

    public event Action? Invalidated;
    public event Action? Loaded;
    public event Action<InventoryItemEventArgs>? ItemAdded;
    public event Action<InventoryItemEventArgs>? ItemUpdated;
    public event Action<InventoryItemEventArgs>? ItemRemoved;

    protected override void OnDisconnected()
    {
        _inventory = null;
        _forceLoadingInventory = false;
        _currentPacketIndex = 0;
        _totalPackets = 0;
    }

    /// <summary>
    /// Returns the inventory immediately if it is available
    /// and has not been invalidated, otherwise attempts to retrieve it from the server.
    /// Note that the user must be in a room to retrieve the inventory from the server.
    /// If the user is not in a room and a request to load the inventory is made, this method will time out.
    /// </summary>
    public async Task<IInventory> GetInventoryAsync(int timeout = XabboConst.DefaultTimeout,
        CancellationToken cancellationToken = default)
    {
        if (_inventory?.IsInvalidated == false)
        {
            return _inventory;
        }
        else
        {
            Task<IInventory> loadTask = _loadTcs.Task;

            CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            if (timeout > 0) cts.CancelAfter(timeout);

            try
            {
                if (!_forceLoadingInventory)
                {
                    Interceptor.Send(Out.RequestFurniInventory);
                    _forceLoadingInventory = true;
                }

                await await Task.WhenAny(loadTask, Task.Delay(Timeout.Infinite, cts.Token));
                return await loadTask;
            }
            finally { cts.Dispose(); }
        }
    }

    private void SetLoadTaskResult(IInventory inventory)
    {
        _loadTcs.TrySetResult(inventory);
        _loadTcs = new TaskCompletionSource<IInventory>(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
