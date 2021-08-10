using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core
{
    /// <summary>
    /// Defines a high score value with a list of associated users.
    /// </summary>
    public interface IHighScore : IComposable
    {
        /// <summary>
        /// The value of this high score.
        /// </summary>
        int Value { get; }
        /// <summary>
        /// The users with this high score.
        /// </summary>
        IReadOnlyList<string> Users { get; }
    }
}
