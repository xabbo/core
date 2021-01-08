using System;

namespace Xabbo.Core.Messages
{
    public static class MessageGroups
    {
        /// <summary>
        /// Indicates that the identifier attributes defined on the class are to be scanned when checking identifiers.
        /// Identifier attributes defined on the class are always required when attaching a listener.
        /// </summary>
        public static readonly object Class = "<Class>";
        /// <summary>
        /// Indicates that the default message group is required when attaching a listener,
        /// or to scan identifier attributes in the default message group (no message group specified)
        /// when checking identifiers / attachment state.
        /// </summary>
        public static readonly object Default = "<Default>";
        /// <summary>
        /// Indicates that all message groups are required when attaching a listener,
        /// or to scan identifier attributes in all message groups when checking identifiers / attachment state.
        /// </summary>
        public static readonly object All = "<All>";
        /// <summary>
        /// Indicates that no message groups are required when attaching a listener.
        /// </summary>
        public static readonly object None = "<None>";
    }
}
