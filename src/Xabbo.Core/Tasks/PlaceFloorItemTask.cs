using System;
using Xabbo.Core.Messages.Incoming;
using Xabbo.Core.Messages.Outgoing;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

/// <summary>
/// Sends a request to place a floor item and returns whether the item was successfully placed.
/// </summary>
/// <param name="interceptor">The interceptor.</param>
/// <param name="itemId">The ID of the item to place.</param>
/// <param name="location">The location to place the item.</param>
/// <param name="direction">The direction to place the item in.</param>
[Intercept]
public sealed partial class PlaceFloorItemTask(
    IInterceptor interceptor, Id itemId, Point location, int direction
)
    : InterceptorTask<PlaceFloorItemTask.Result>(interceptor)
{
    public enum Result { Error, Success }
    const string FurniPlacementError = "furni_placement_error";

    public Id ItemId { get; } = itemId;
    public Point Location { get; } = location;
    public int Direction { get; } = direction;

    protected override void OnExecute() => Interceptor.Send(new PlaceFloorItemMsg(ItemId, Location, Direction));

    [Intercept]
    void HandleFloorItemAdded(FloorItemAddedMsg msg)
    {
        if (msg.Item.Id == Math.Abs(ItemId))
            SetResult(Result.Success);
    }

    // This is also received on Shockwave, but there is no mapping for the header.
    [Intercept]
    void HandleNotificationDialog(NotificationDialogMsg msg)
    {
        if (msg.Type == FurniPlacementError)
            SetResult(Result.Error);
    }
}