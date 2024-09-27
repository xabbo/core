namespace Xabbo.Core;

/// <summary>
/// Represents a crackable furni state.
/// </summary>
public interface ICrackableFurniData : IItemData
{
    /// <summary>
    /// The number of hits the crackable furni has taken.
    /// </summary>
    int Hits { get; }

    /// <summary>
    /// The target hit count of the crackable furni.
    /// </summary>
    int Target { get; }
}
