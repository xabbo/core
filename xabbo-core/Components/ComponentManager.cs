using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Components
{
    public class ComponentManager
    {
        private static readonly Type componentType = typeof(XabboComponent);

        private static void EnsureIsComponent(Type type)
        {
            if (!type.IsSubclassOf(componentType))
                throw new ArgumentException($"The specified type must be a subclass of {componentType.FullName}");
        }

        public IInterceptor Interceptor { get; }

        private readonly ConcurrentDictionary<Type, XabboComponent> components = new ConcurrentDictionary<Type, XabboComponent>();
        private readonly HashSet<Type> faultedComponents = new HashSet<Type>();

        public ComponentManager(IInterceptor interceptor)
        {
            Interceptor = interceptor;
        }

        private bool CheckComponent(Type type)
        {
            lock (faultedComponents)
            {
                if (faultedComponents.Contains(type))
                    return false;

                if (!Interceptor.Dispatcher.Headers.AreResolved(type, MessageGroups.Default))
                {
                    faultedComponents.Add(type);
                    return false;
                }

                return true;
            }
        }

        public bool IsAvailable<T>() where T : XabboComponent => IsAvailable(typeof(T));
        public bool IsAvailable(Type componentType)
        {
            EnsureIsComponent(componentType);
            return components.ContainsKey(componentType);
        }

        public bool IsAvailable<T>(object messageGroup) where T : XabboComponent => IsAvailable(typeof(T), messageGroup);
        public bool IsAvailable(Type componentType, object messageGroup)
        {
            EnsureIsComponent(componentType);
            return
                components.ContainsKey(componentType) &&
                components.TryGetValue(componentType, out XabboComponent component) &&
                Interceptor.Dispatcher.IsListenerAttached(component, messageGroup);
        }

        public bool LoadComponent<T>() where T : XabboComponent => LoadComponent(typeof(T));
        public bool LoadComponent(Type componentType) => LoadComponent(componentType, out _);

        public bool LoadComponent<T>(out T component) where T : XabboComponent
        {
            if (LoadComponent(typeof(T), out XabboComponent c))
            {
                component = (T)c;
                return true;
            }
            else
            {
                component = null;
                return false;
            }
        }

        public bool LoadComponent(Type componentType, out XabboComponent component)
        {
            component = null;

            EnsureIsComponent(componentType);

            if (IsAvailable(componentType)) return true;

            if (!CheckComponent(componentType)) return false;

            var dependsOnAttribute = componentType.GetCustomAttribute<DependenciesAttribute>();
            if (dependsOnAttribute != null)
            {
                foreach (var dependency in dependsOnAttribute.Types)
                {
                    if (IsAvailable(dependency)) continue;
                    if (!LoadComponent(dependency)) return false;
                }
            }

            var constructor = componentType.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw new InvalidOperationException($"The component {componentType.FullName} must have a parameterless constructor to be loaded.");

            var c = (XabboComponent)FormatterServices.GetUninitializedObject(componentType);
            c.Manager = this;

            constructor.Invoke(c, null);

            Interceptor.Dispatcher.AttachListener(c);
            if (!components.TryAdd(componentType, c))
            {
                Interceptor.Dispatcher.DetachListener(c);
                return false;
            }

            c.Initialize();

            component = c;
            return true;
        }

        public T GetComponent<T>() where T : XabboComponent
            => components.TryGetValue(typeof(T), out XabboComponent component) ? (T)component : null;

        public XabboComponent GetComponent(Type componentType)
        {
            EnsureIsComponent(componentType);
            return components.TryGetValue(componentType, out XabboComponent component) ? component : null;
        }
    }
}
