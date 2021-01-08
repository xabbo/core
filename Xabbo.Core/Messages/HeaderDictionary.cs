using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Xabbo.Core.Messages
{
    public class HeaderDictionary
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private readonly Type type;
        private readonly Dictionary<string, short> defaultValues = new Dictionary<string, short>();

        private readonly Dictionary<short, Header> _valueMap = new Dictionary<short, Header>();
        private readonly Dictionary<string, Header> _nameMap = new Dictionary<string, Header>();

        public Destination Destination { get; }

        public HeaderDictionary(Destination destination)
        {
            Destination = destination;

            type = GetType();
            InitializeProperties();
        }

        public HeaderDictionary(Destination destination, IReadOnlyDictionary<string, short> values)
            : this(destination)
        {
            if (values == null) throw new ArgumentNullException("values");

            ResetProperties();
            Load(values);
        }

        private void InitializeProperties()
        {
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (prop.PropertyType.Equals(typeof(Header)) &&
                    prop.GetMethod.GetParameters().Length == 0)
                {
                    var attribute = prop.GetCustomAttribute<DefaultValueAttribute>();
                    if (attribute != null)
                    {
                        if (attribute.Value is short s)
                            defaultValues[prop.Name] = s;
                        else if (attribute.Value is int i)
                            defaultValues[prop.Name] = (short)i;
                    }
                }
            }
        }

        private void ResetProperties()
        {
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (prop.PropertyType.Equals(typeof(Header)) &&
                    prop.GetMethod.GetParameters().Length == 0)
                {
                    short value = -1;
                    if (defaultValues.ContainsKey(prop.Name))
                        value = defaultValues[prop.Name];

                    var header = new Header(Destination, prop.Name, value);
                    _nameMap[prop.Name] = header;
                    if (value > -1) _valueMap[value] = header;

                    prop.SetValue(this, header);
                }
            }
        }

        public void Load(IReadOnlyDictionary<string, short> values)
        {
            var identifierSet = new HashSet<string>();
            var headerSet = new HashSet<short>();

            foreach (var pair in values)
            {
                if (!identifierSet.Add(pair.Key))
                    throw new InvalidOperationException($"Attempting to load duplicate identifier '{pair.Key}'.");
                if (pair.Value >= 0 && !headerSet.Add(pair.Value))
                {
                    // throw new InvalidOperationException($"Attempting to load duplicate header {pair.Value} for identifier '{pair.Key}'.");
                    DebugUtil.Log($"loading duplicate header {pair.Value} for identifier '{pair.Key}'");
                }
            }

            _lock.EnterWriteLock();
            try
            {
                _valueMap.Clear();
                _nameMap.Clear();

                ResetProperties();

                foreach (var pair in values)
                {
                    var header = new Header(Destination, pair.Key, pair.Value);

                    _nameMap[pair.Key] = header;
                    if (pair.Value >= 0)
                        _valueMap[pair.Value] = header;

                    var prop = type.GetProperty(pair.Key, typeof(Header));
                    if (prop != null)
                        prop.SetValue(this, new Header(Destination, prop.Name, pair.Value));
                }
            }
            finally { _lock.ExitWriteLock(); }
        }

        public bool HasIdentifier(string identifier)
        {
            _lock.EnterReadLock();
            try { return _nameMap.ContainsKey(identifier); }
            finally { _lock.ExitReadLock(); }
        }

        public bool TryGetIdentifier(short id, out string name)
        {
            _lock.EnterReadLock();
            try
            {
                name = null;
                if (!_valueMap.TryGetValue(id, out Header header))
                    return false;

                name = header.Name;
                return true;
            }
            finally { _lock.ExitReadLock(); }
        }

        public bool TryGetHeader(short id, out Header header)
        {
            _lock.EnterReadLock();
            try { return _valueMap.TryGetValue(id, out header); }
            finally { _lock.ExitReadLock(); }
        }

        public bool TryGetHeader(string identifier, out Header header)
        {
            _lock.EnterReadLock();
            try { return _nameMap.TryGetValue(identifier, out header); }
            finally { _lock.ExitReadLock(); }
        }

        public Header this[string identifier]
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    if (_nameMap.TryGetValue(identifier, out Header header))
                        return header;
                    else
                        return new Header(Destination, identifier, -1);
                }
                finally { _lock.ExitReadLock(); }
            }
        }
    }
}
