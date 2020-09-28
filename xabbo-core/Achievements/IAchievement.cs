using System;

namespace Xabbo.Core
{
    public interface IAchievement
    {
        int Id { get; }
        int Level { get; }
        string BadgeName { get; }
        int BaseProgress { get; }
        int MaxProgress { get; }
        int CurrentProgress { get; }
        bool IsCompleted { get; }
        string Category { get; }
        int MaxLevel { get; }
    }
}
