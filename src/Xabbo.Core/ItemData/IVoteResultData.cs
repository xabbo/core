namespace Xabbo.Core;

/// <summary>
/// Represents the result of a vote.
/// </summary>
public interface IVoteResultData : IItemData
{
    int Result { get; }
}
