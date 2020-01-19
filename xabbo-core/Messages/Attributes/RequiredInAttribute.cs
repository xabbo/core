using System;

namespace Xabbo.Core.Messages
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class RequiredInAttribute : IdentifiersAttribute
    {
        public RequiredInAttribute(params string[] identifiers)
            : base(Destination.Client, identifiers)
        { }
    }
}
