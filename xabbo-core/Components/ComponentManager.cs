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

                if (!Interceptor.Dispatcher.Headers.AreResolved(type, MessageGroups.Class, MessageGroups.Default))
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
            return 
                components.TryGetValue(componentType, out XabboComponent component) &&
                component.IsAvailable;
        }
        
        public bool IsAvailable<T>(params object[] messageGroups) where T : XabboComponent => IsAvailable(typeof(T), messageGroups);
        public bool IsAvailable(Type componentType, params object[] messageGroups)
        {
            EnsureIsComponent(componentType);
            return
                components.TryGetValue(componentType, out XabboComponent component) &&
                Interceptor.Dispatcher.IsAttached(component, messageGroups);
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

            XabboComponent c;
            
            if (!components.TryGetValue(componentType, out c))
            {
                var constructor = componentType.GetConstructor(Type.EmptyTypes);
                if (constructor == null)
                    throw new InvalidOperationException($"The component {componentType.FullName} must have a parameterless constructor to be loaded.");

                c = (XabboComponent)FormatterServices.GetUninitializedObject(componentType);
                c.Manager = this;

                constructor.Invoke(c, null);

                if (!components.TryAdd(componentType, c))
                    return false;
            }

            try { Interceptor.Dispatcher.Attach(c); }
            catch
            {
                components.TryRemove(componentType, out _);
                c.Manager = null;
                return false;
            }

            c.Initialize();
            c.IsAvailable = true;

            component = c;
            return true;
        }

        public void LoadComponents(IEnumerable<Type> componentTypes)
        {
            foreach (var type in componentTypes)
                LoadComponent(type);
        }

        public bool LoadComponent(XabboComponent component)
        {
            LoadComponents(new XabboComponent[] { component });
            return component.IsAvailable;
        }

        public void LoadComponents(IEnumerable<XabboComponent> components)
        {
            // Ensure all components are attached to this manager
            foreach (var component in components)
            {
                var type = component.GetType();
                if (component.Manager == null)
                {
                    if (!this.components.TryAdd(component.GetType(), component))
                        throw new Exception($"Unable to add component {type.FullName} to dictionary");
                }
                else
                {
                    if (!ReferenceEquals(component.Manager, this))
                        throw new InvalidOperationException($"The component {type.FullName} belongs to another component manager.");
                }
            }

            foreach (var component in components)
            {
                LoadComponent(component.GetType());
            }
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
