using System;
using System.Reflection;
using System.Text;

namespace Xabbo.Core.Messages
{
    public class UnknownIdentifierException : Exception
    {
        public Identifier Identifier { get; }
        public MemberInfo Member { get; }

        public UnknownIdentifierException(Identifier identifier)
            : this(identifier, null)
        { }

        public UnknownIdentifierException(Identifier identifier, MemberInfo member)
            : base(ConstructMessage(identifier, member))
        {
            Identifier = identifier;
            Member = member;
        }

        private static string ConstructMessage(Identifier identifier, MemberInfo member)
        {
            var sb = new StringBuilder();
            sb.Append("Unknown ");
            sb.Append(identifier.IsOutgoing ? "outgoing" : "incoming");
            sb.Append(" identifier '");
            sb.Append(identifier.Name);
            sb.Append("'");

            if (member != null)
            {
                switch (member.MemberType)
                {
                    case MemberTypes.TypeInfo:
                    case MemberTypes.NestedType:
                        sb.Append(" defined in attribute for class ");
                        sb.Append(member.Name);
                        break;
                    case MemberTypes.Method:
                        sb.Append(" defined in attribute for method ");
                        sb.Append(member.DeclaringType.Name);
                        sb.Append(".");
                        sb.Append(member.Name);
                        break;
                    case MemberTypes.Constructor:
                    case MemberTypes.Event:
                    case MemberTypes.Field:
                    case MemberTypes.Property:
                    case MemberTypes.Custom:
                    case MemberTypes.All:
                    default: throw new Exception($"Invalid member type {member.MemberType}");
                }
            }

            return sb.ToString();
        }
    }
}
