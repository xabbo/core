using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xabbo.Core.Messages
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class RequiredIdentifiersAttribute : IdentifiersAttribute
    {
        public RequiredIdentifiersAttribute(params string[] identifiers)
            : base(Destination.Client, identifiers)
        { }
    }
}
