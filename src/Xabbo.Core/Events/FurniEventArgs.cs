using System;

namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for events involving a single <see cref="IFurni"/>.
/// </summary>
/// <param name="furni">The furni involved in the event.</param>
public class FurniEventArgs(IFurni furni) : EventArgs
{
    /// <summary>
    /// Gets the furni involved in the event.
    /// </summary>
    public IFurni Furni { get; } = furni;
}
