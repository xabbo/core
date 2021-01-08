using System;

namespace Xabbo.Core.Messages
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequiredOutAttribute : IdentifiersAttribute
    {
        public RequiredOutAttribute(params string[] identifiers)
            : base(Destination.Server, identifiers)
        { }
    }
}
