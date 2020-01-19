using System;
using System.Reflection;

namespace Xabbo.Core.Messages
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ReceiveAttribute : IdentifiersAttribute
    {
        public ReceiveAttribute(params string[] identifiers)
            : base(Destination.Client, identifiers)
        { }
    }
}
