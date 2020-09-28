using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xabbo.Core.Messages
{
    public class Headers
    {
        private static IEnumerable<MethodInfo> FindAllMethods(Type type)
        {
            IEnumerable<MethodInfo> methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (type.BaseType != null)
                methods = methods.Concat(FindAllMethods(type.BaseType));

            return methods;
        }

        public IncomingHeaders Incoming { get; }
        public OutgoingHeaders Outgoing { get; }

        public short this[Identifier identifier]
        {
            get
            {
                if (identifier.Destination == Destination.Server)
                    return Outgoing[identifier.Name];
                else if (identifier.Destination == Destination.Client)
                    return Incoming[identifier.Name];
                else
                    throw new Exception($"Invalid identifier destination: {identifier.Destination}");
            }
        }

        public Headers()
        {
            Incoming = new IncomingHeaders();
            Outgoing = new OutgoingHeaders();
        }

        public Headers(IncomingHeaders incoming, OutgoingHeaders outgoing)
        {
            Incoming = incoming;
            Outgoing = outgoing;
        }

        public void Load(IDictionary<string, short> incoming, IDictionary<string, short> outgoing)
        {
            Incoming.Load(incoming);
            Outgoing.Load(outgoing);
        }

        private HeaderDictionary GetDictionary(Destination destination)
            => destination == Destination.Server ? Outgoing : (HeaderDictionary)Incoming;
        private bool CheckIdentifierDefined(Identifier identifier)
            => GetDictionary(identifier.Destination).HasIdentifier(identifier.Name);
        private bool CheckIdentifierResolved(Identifier identifier)
            => GetDictionary(identifier.Destination).TryGetHeader(identifier.Name, out Header header) && header >= 0;

        public bool HasIdentifier(Identifier identifier) 
            => GetDictionary(identifier.Destination).HasIdentifier(identifier.Name);

        public bool HasIdentifier(Destination destination, string identifier)
            => GetDictionary(destination).HasIdentifier(identifier);

        public bool TryGetHeader(Identifier identifier, out Header header)
            => GetDictionary(identifier.Destination).TryGetHeader(identifier.Name, out header);

        public bool TryGetHeader(Destination destination, string identifier, out Header header)
            => GetDictionary(destination).TryGetHeader(identifier, out header);

        public bool IsResolved(Destination destination, string identifier)
        {
            if (!HasIdentifier(destination, identifier))
                throw new UnknownIdentifierException(new Identifier(destination, identifier));
            return TryGetHeader(destination, identifier, out Header header) && header >= 0;
        }

        public bool IsResolved(Identifier identifier)
        {
            if (!HasIdentifier(identifier))
                throw new Exception($"Unknown {(identifier.IsOutgoing ? "outgoing" : "incoming")} identifier '{identifier.Name}'");
            return TryGetHeader(identifier, out Header header) && header >= 0;
        }

        public bool AreResolved(IEnumerable<Identifier> identifiers)
            => identifiers.All(identifier => IsResolved(identifier));

        public bool AreResolved(Destination destination, IEnumerable<string> identifiers)
        {
            foreach (var identifier in identifiers)
            {
                if (!HasIdentifier(destination, identifier))
                    throw new Exception($"Unknown {(destination == Destination.Server ? "outgoing" : "incoming")} identifier '{identifier}'");
                if (!TryGetHeader(destination, identifier, out Header header) || header < 0) return false;
            }

            return true;
        }

        public bool AreResolvedIn(params string[] identifiers) => AreResolved(Destination.Client, identifiers);
        public bool AreResolvedOut(params string[] identifiers) => AreResolved(Destination.Server, identifiers);

        public bool AreResolved<T>(params object[] targetGroups) where T : class
            => !GetUnresolvedIdentifiers(typeof(T), targetGroups).Any();

        public bool AreResolved(object target, params object[] targetGroups)
            => !GetUnresolvedIdentifiers(target.GetType(), targetGroups).Any();

        public bool AreResolved(Type type, params object[] targetGroups)
            => !GetUnresolvedIdentifiers(type, targetGroups).Any();

        public Identifiers GetUnresolvedIdentifiers(Identifiers identifiers)
        {
            var unresolved = new Identifiers();
            foreach (var identifier in identifiers)
            {
                if (!IsResolved(identifier))
                    unresolved.Add(identifier);
            }
            return unresolved;
        }

        public Identifiers GetUnresolvedIdentifiers<T>(params object[] targetGroups) where T : class
            => GetUnresolvedIdentifiers(typeof(T), targetGroups);

        public Identifiers GetUnresolvedIdentifiers(object target, params object[] targetGroups)
            => GetUnresolvedIdentifiers(target.GetType(), targetGroups);

        public Identifiers GetUnresolvedIdentifiers(Type type, params object[] targetGroups)
        {
            if (targetGroups == null)
                throw new ArgumentNullException("targetGroups");

            bool checkAll = targetGroups.Length == 0 ||
                (targetGroups.Length == 1 && targetGroups[0] == MessageGroups.All);
            bool checkClass = targetGroups.Contains(MessageGroups.Class);

            var ids = new Identifiers();

            if (checkAll || checkClass)
            {
                foreach (var attr in type.GetCustomAttributes<IdentifiersAttribute>())
                {
                    foreach (var identifier in attr.Identifiers)
                    {
                        if (!CheckIdentifierDefined(identifier))
                            throw new UnknownIdentifierException(identifier, type);
                        if (!CheckIdentifierResolved(identifier))
                        {
                            ids.Add(identifier);
                        }
                    }
                }
            }

            foreach (var method in FindAllMethods(type))
            {
                var groupAttribute = method.GetCustomAttribute<GroupAttribute>();
                object[] tags;
                if (groupAttribute != null && groupAttribute.Tags.Any())
                    tags = groupAttribute.Tags;
                else
                    tags = new object[] { MessageGroups.Default };

                foreach (var attr in method.GetCustomAttributes<IdentifiersAttribute>())
                {
                    if (!checkAll && !targetGroups.Any(group => tags.Contains(group)))
                        continue;

                    foreach (var identifier in attr.Identifiers)
                    {
                        if (!CheckIdentifierDefined(identifier))
                            throw new UnknownIdentifierException(identifier, type);
                        if (!CheckIdentifierResolved(identifier))
                            ids.Add(identifier);
                    }
                }
            }

            return ids;
        }
    }
}
