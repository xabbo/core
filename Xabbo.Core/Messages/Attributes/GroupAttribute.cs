using System;

namespace Xabbo.Core.Messages
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class GroupAttribute : Attribute
    {
        public object[] Tags { get; }

        public GroupAttribute(params object[] tags)
        {
            Tags = tags;
        }
    }
}
