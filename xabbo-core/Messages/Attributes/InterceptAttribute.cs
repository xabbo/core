using System;

namespace Xabbo.Core.Messages
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class InterceptAttribute : IdentifiersAttribute
    {
        internal InterceptAttribute(Destination destination, string[] identifiers)
            : base(destination, identifiers)
        { }
    }

    public sealed class InterceptInAttribute : InterceptAttribute
    {
        public InterceptInAttribute(params string[] identifiers)
            : base(Destination.Client, identifiers)
        { }
    }

    public sealed class InterceptOutAttribute : InterceptAttribute
    {
        public InterceptOutAttribute(params string[] identifiers)
            : base(Destination.Server, identifiers)
        { }
    }
}
