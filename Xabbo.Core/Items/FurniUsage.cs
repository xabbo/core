using System;

namespace Xabbo.Core;

public enum FurniUsage
{
    /// <summary>
    /// The furni cannot be used
    /// </summary>
    None = 0,
    /// <summary>
    /// Users with rights can use the furni
    /// </summary>
    Rights = 1,
    /// <summary>
    /// Anyone can use the furni
    /// </summary>
    Anyone = 2
}
