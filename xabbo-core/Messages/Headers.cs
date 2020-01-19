using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xabbo.Core.Messages
{
    public class Headers<TIncoming, TOutgoing>
        where TIncoming : HeaderDictionary
        where TOutgoing : HeaderDictionary
    {
        private static IEnumerable<MethodInfo> FindAllMethods(Type type)
        {
            IEnumerable<MethodInfo> methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (type.BaseType != null)
                methods = methods.Concat(FindAllMethods(type.BaseType));

            return methods;
        }

        public TIncoming Incoming { get; }
        public TOutgoing Outgoing { get; }

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

        public Headers(TIncoming incoming, TOutgoing outgoing)
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
            => GetDictionary(identifier.Destination).TryGetHeader(identifier.Name, out short header) && header >= 0;

        public bool HasIdentifier(Identifier identifier) => GetDictionary(identifier.Destination).HasIdentifier(identifier.Name);
        public bool HasIdentifier(Destination destination, string identifier) => GetDictionary(destination).HasIdentifier(identifier);
        public bool TryGetHeader(Identifier identifier, out short header) => GetDictionary(identifier.Destination).TryGetHeader(identifier.Name, out header);
        public bool TryGetHeader(Destination destination, string identifier, out short header) => GetDictionary(destination).TryGetHeader(identifier, out header);

        public bool IsResolved(Destination destination, string identifier)
        {
            if (!HasIdentifier(destination, identifier))
                throw new UnknownIdentifierException(new Identifier(destination, identifier));
            return TryGetHeader(destination, identifier, out short header) && header >= 0;
        }
        public bool IsResolved(Identifier identifier)
        {
            if (!HasIdentifier(identifier))
                throw new Exception($"Unknown {(identifier.IsOutgoing ? "outgoing" : "incoming")} identifier '{identifier.Name}'");
            return TryGetHeader(identifier, out short header) && header >= 0;
        }
        public bool AreResolved(IEnumerable<Identifier> identifiers)
            => identifiers.All(identifier => IsResolved(identifier));
        public bool AreResolved(Destination destination, IEnumerable<string> identifiers)
        {
            foreach (var identifier in identifiers)
            {
                if (!HasIdentifier(destination, identifier))
                    throw new Exception($"Unknown {(destination == Destination.Server ? "outgoing" : "incoming")} identifier '{identifier}'");
                if (!TryGetHeader(destination, identifier, out short header) || header < 0) return false;
            }

            return true;
        }
        public bool AreResolvedIn(IEnumerable<string> identifiers) => AreResolved(Destination.Client, identifiers);
        public bool AreResolvedIn(params string[] identifiers) => AreResolved(Destination.Client, identifiers);
        public bool AreResolvedOut(IEnumerable<string> identifiers) => AreResolved(Destination.Server, identifiers);
        public bool AreResolvedOut(params string[] identifiers) => AreResolved(Destination.Server, identifiers);

        public bool AreResolved<T>() where T : class => AreResolved<T>(MessageGroups.All);
        public bool AreResolved<T>(object withTag) where T : class => !GetUnresolvedIdentifiers(typeof(T), withTag).Any();
        public bool AreResolved(object target) => AreResolved(target, MessageGroups.All);
        public bool AreResolved(object target, object withTag) => !GetUnresolvedIdentifiers(target.GetType(), withTag).Any();
        public bool AreResolved(Type type) => AreResolved(type, MessageGroups.All);
        public bool AreResolved(Type type, object withTag) => !GetUnresolvedIdentifiers(type, withTag).Any();

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
        public Identifiers GetUnresolvedIdentifiers<T>() where T : class => GetUnresolvedIdentifiers<T>(MessageGroups.All);
        public Identifiers GetUnresolvedIdentifiers<T>(object withTag) where T : class => GetUnresolvedIdentifiers(typeof(T), withTag);
        public Identifiers GetUnresolvedIdentifiers(object target) => GetUnresolvedIdentifiers(target, MessageGroups.All);
        public Identifiers GetUnresolvedIdentifiers(object target, object withTag) => GetUnresolvedIdentifiers(target.GetType(), withTag);
        public Identifiers GetUnresolvedIdentifiers(Type type) => GetUnresolvedIdentifiers(type, MessageGroups.All);

        public Identifiers GetUnresolvedIdentifiers(Type type, object withTag)
        {
            var ids = new Identifiers();

            bool searchAll = ReferenceEquals(withTag, MessageGroups.All);
            bool searchClassAttributesOnly = ReferenceEquals(withTag, MessageGroups.ClassOnly);

            // if (searchAll || searchClassAttributesOnly)
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

            if (searchClassAttributesOnly)
                return ids;

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
                    if (!searchAll && !tags.Any(tag => Equals(tag, withTag))) continue;
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

    public class Headers : Headers<IncomingHeaders, OutgoingHeaders>
    {
        public Headers() : base(new IncomingHeaders(), new OutgoingHeaders()) { }
    }
}
