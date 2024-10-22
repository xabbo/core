using System;
using Xabbo.Core.Messages.Incoming;
using Xabbo.Core.Messages.Outgoing;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

/// <summary>
/// Sends a request to place a wall item and returns whether the item was successfully placed.
/// </summary>
/// <param name="interceptor">The interceptor.</param>
/// <param name="itemId">The ID of the item to place.</param>
/// <param name="location">The location to place the item.</param>
[Intercept]
public sealed partial class PlaceWallItemTask(
    IInterceptor interceptor, Id itemId, WallLocation location
)
    : InterceptorTask<PlaceWallItemTask.Result>(interceptor)
{
    public enum Result { Error, Success }

    public Id ItemId { get; } = itemId;
    public WallLocation Location { get; } = location;

    protected override void OnExecute() => Interceptor.Send(new PlaceWallItemMsg(ItemId, Location));

    [Intercept]
    void HandleWallItemAdded(WallItemAddedMsg msg)
    {
        if (msg.Item.Id == Math.Abs(ItemId))
            SetResult(Result.Success);
    }
}