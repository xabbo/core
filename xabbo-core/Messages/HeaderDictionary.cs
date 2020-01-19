using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace Xabbo.Core.Messages
{
    public class HeaderDictionary
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private readonly Type type;
        private readonly Dictionary<string, short> defaultValues = new Dictionary<string, short>();
        private readonly Dictionary<string, short> valueMap = new Dictionary<string, short>();
        private readonly Dictionary<short, string> nameMap = new Dictionary<short, string>();

        public HeaderDictionary()
        {
            type = GetType();
            InitializeProperties();
        }

        public HeaderDictionary(IDictionary<string, short> values)
            : this()
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
                if (prop.PropertyType.Equals(typeof(short)) &&
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
                if (prop.PropertyType.Equals(typeof(short)) &&
                    prop.GetMethod.GetParameters().Length == 0)
                {
                    short value = -1;
                    if (defaultValues.ContainsKey(prop.Name))
                        value = defaultValues[prop.Name];
                    prop.SetValue(this, value);
                    valueMap[prop.Name] = value;
                }
            }
        }

        public void Load(IDictionary<string, short> values)
        {
            var identifierSet = new HashSet<string>();
            var headerSet = new HashSet<short>();

            foreach (var pair in values)
            {
                if (!identifierSet.Add(pair.Key))
                    throw new InvalidOperationException($"Trying to load duplicate identifier '{pair.Key}'.");
                if (pair.Value >= 0 && !headerSet.Add(pair.Value))
                    throw new InvalidOperationException($"Trying to load duplicate header {pair.Value} for identifier '{pair.Key}'.");
            }

            _lock.EnterWriteLock();
            try
            {
                valueMap.Clear();
                nameMap.Clear();

                ResetProperties();
                // TODO threads could read *properties*
                // that have just been reset ?

                foreach (var pair in values)
                {
                    valueMap[pair.Key] = pair.Value;
                    if (pair.Value >= 0) nameMap[pair.Value] = pair.Key;

                    var prop = type.GetProperty(pair.Key, typeof(short));
                    if (prop != null)
                    {
                        prop.SetValue(this, pair.Value);
                    }
                }
            }
            finally { _lock.ExitWriteLock(); }
        }

        public bool HasIdentifier(string identifier)
        {
            _lock.EnterReadLock();
            try { return valueMap.ContainsKey(identifier); }
            finally { _lock.ExitReadLock(); }
        }

        public bool TryGetIdentifier(short id, out string name)
        {
            _lock.EnterReadLock();
            try { return nameMap.TryGetValue(id, out name); }
            finally { _lock.ExitReadLock(); }
        }

        public bool TryGetHeader(string identifier, out short header)
        {
            _lock.EnterReadLock();
            try
            {
                if (valueMap.TryGetValue(identifier, out header))
                {
                    return true;
                }
                else
                {
                    header = -1;
                    return false;
                }
            }
            finally { _lock.ExitReadLock(); }
        }

        public short this[string identifier]
        {
            get
            {
                _lock.EnterReadLock();
                try { return valueMap.TryGetValue(identifier, out short header) ? header : (short)-1; }
                finally { _lock.ExitReadLock(); }
            }

            /*set
            {
                _lock.EnterWriteLock();
                try
                {
                    bool hasPreviousValue = valueMap.TryGetValue(identifier, out short previousValue);
                    if (hasPreviousValue && value == previousValue) return;

                    if (value >= 0 &&
                        nameMap.TryGetValue(value, out string name) &&
                        !string.Equals(identifier, name))
                    {
                        throw new Exception($"Cannot set identifier '{identifier}', another identifier '{name}' has the value {value}");
                    }

                    valueMap[identifier] = value;
                    var prop = type.GetProperty(identifier, typeof(short));
                    if (prop != null) prop.SetValue(this, value);

                    if (hasPreviousValue && previousValue >= 0) nameMap.Remove(previousValue);
                    if (value >= 0) nameMap[value] = identifier;
                }
                finally { _lock.ExitWriteLock(); }
            }*/
        }
    }
}
