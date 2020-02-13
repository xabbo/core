using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Xabbo.Core.Protocol;

namespace Xabbo.Core.Messages
{
    public class MessageDispatcher<THeaders, TIn, TOut, TSender>
        where THeaders : Headers<TIn, TOut>
        where TIn : HeaderDictionary
        where TOut : HeaderDictionary
    {
        private static ReceiveCallback<TSender> CreateCallback(short header, object target, object[] tags, MethodInfo method)
        {
            var callback = ReceiveDelegateFactory<TSender>.GetOpenDelegate(method);
            return new OpenReceiverCallback<TSender>(header, target, tags, callback);
        }

        private static InterceptCallback CreateInterceptCallback(Destination destination, short header,
            object target, object[] tags, MethodInfo method)
        {
            var callback = InterceptDelegateFactory.GetOpenDelegate(method);
            return new OpenInterceptCallback(destination, header, target, method, tags, callback);
        }
        
        private static IReadOnlyList<ReceiveCallback<TSender>> ReceiverCallbackListFactory(short _)
            => new List<ReceiveCallback<TSender>>();
        private static IReadOnlyList<InterceptCallback> InterceptCallbackListFactory(short _)
            => new List<InterceptCallback>();

        private ConcurrentDictionary<IListener, IReadOnlyList<ListenerCallback>> listeners;
        private ConcurrentDictionary<short, IReadOnlyList<ReceiveCallback<TSender>>> receiveCallbacks;
        private ConcurrentDictionary<short, IReadOnlyList<InterceptCallback>> incomingInterceptCallbacks;
        private ConcurrentDictionary<short, IReadOnlyList<InterceptCallback>> outgoingInterceptCallbacks;

        private ConcurrentDictionary<short, IReadOnlyList<InterceptCallback>> GetInterceptCallbackDictionary(Destination destination)
            => destination == Destination.Server ? outgoingInterceptCallbacks : incomingInterceptCallbacks;

        public THeaders Headers { get; }

        public MessageDispatcher(THeaders headers)
        {
            Headers = headers;

            listeners = new ConcurrentDictionary<IListener, IReadOnlyList<ListenerCallback>>();
            receiveCallbacks = new ConcurrentDictionary<short, IReadOnlyList<ReceiveCallback<TSender>>>();
            incomingInterceptCallbacks = new ConcurrentDictionary<short, IReadOnlyList<InterceptCallback>>();
            outgoingInterceptCallbacks = new ConcurrentDictionary<short, IReadOnlyList<InterceptCallback>>();
        }

        private bool IsValidParameter(ParameterInfo param)
        {
            return !param.IsOut && !param.IsIn && !param.IsOptional && !param.HasDefaultValue;
        }

        private bool CheckReceiveMethodSignature(MethodInfo methodInfo)
        {
            if (!methodInfo.ReturnType.Equals(typeof(void)))
                return false;

            var parameters = methodInfo.GetParameters();
            if (!parameters.All(param => IsValidParameter(param)))
                return false;

            switch (parameters.Length)
            {
                case 0: return true;
                case 1:
                    return
                        parameters[0].ParameterType.IsAssignableFrom(typeof(TSender)) ||
                        parameters[0].ParameterType.Equals(typeof(Packet));
                case 2:
                    return
                        parameters[0].ParameterType.IsAssignableFrom(typeof(TSender)) &&
                        parameters[1].ParameterType.Equals(typeof(Packet));
                default: return false;
            }
        }

        private bool CheckInterceptorMethodSignature(MethodInfo methodInfo)
        {
            if (!methodInfo.ReturnType.Equals(typeof(void)))
                return false;

            var parameters = methodInfo.GetParameters();
            return
                parameters.Length == 1 &&
                parameters[0].ParameterType.Equals(typeof(InterceptEventArgs)) &&
                parameters.All(param => IsValidParameter(param));
        }

        public void DispatchMessage(TSender sender, Packet packet)
        {
            IReadOnlyList<ReceiveCallback<TSender>> list;

            // global callbacks
            if (receiveCallbacks.TryGetValue(-1, out list))
                InvokeReceiverCallbacks(list, sender, packet);

            // bound callbacks
            if (receiveCallbacks.TryGetValue(packet.Header, out list))
                InvokeReceiverCallbacks(list, sender, packet);
        }

        private void InvokeReceiverCallbacks(IEnumerable<ReceiveCallback<TSender>> callbacks, TSender sender, Packet packet)
        {
            short header = packet.Header;

            foreach (var callback in callbacks)
            {
                try
                {
                    packet.Position = 0;
                    callback.Invoke(sender, packet);
                }
                catch (Exception ex)
                {
                    while (ex is TargetInvocationException)
                        ex = ex.InnerException;

                    string messageName = Headers.Incoming.TryGetIdentifier(header, out string name) ? $"'{name}' ({header})" : $"{header}";
                    Debug.WriteLine(
                        $"[MessageDispatcher] Unhandled exception occurred in receiver method " +
                        $"{callback.Target.GetType().FullName}.{callback.Method.Name} " +
                        // TODO the method name here is incorrect due to being generated
                        $"for message {messageName}: {ex.Message}\r\n{ex.StackTrace}"
                    );
                }
            }
        }

        public void DispatchIntercept(InterceptEventArgs e)
        {
            var dict = GetInterceptCallbackDictionary(e.Destination);

            IReadOnlyList<InterceptCallback> list;

            if (dict.TryGetValue(e.Packet.Header, out list))
                InvokeInterceptCallbacks(list, e);
        }

        private void InvokeInterceptCallbacks(IEnumerable<InterceptCallback> callbacks, InterceptEventArgs e)
        {
            short header = e.Packet.Header;

            foreach (var callback in callbacks)
            {
                try
                {
                    e.Packet.Position = 0;
                    callback.Invoke(e);
                }
                catch (Exception ex)
                {
                    while (ex is TargetInvocationException)
                        ex = ex.InnerException;

                    var map = e.IsOutgoing ? (HeaderDictionary)Headers.Outgoing : Headers.Incoming;
                    string messageName = map.TryGetIdentifier(header, out string name) ? $"{name} ({header})" : $"{header}";
                    Debug.WriteLine(
                        $"[MessageDispatcher] Unhandled exception occurred in interceptor method " +
                        $"{callback.Target.GetType().FullName}.{callback.Method.Name} " +
                        $"for message {messageName}: {ex.Message}\r\n{ex.StackTrace}"
                    );
                }
            }
        }

        #region - Handlers -
        public bool AddHandler(short header, Action<TSender, Packet> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            bool result;
            IReadOnlyList<ReceiveCallback<TSender>> previousList, newList;

            do
            {
                previousList = receiveCallbacks.GetOrAdd(header, ReceiverCallbackListFactory);

                if (previousList.Any(callback => handler.Equals(callback.Delegate)))
                {
                    newList = previousList;

                    result = false;
                }
                else
                {
                    var list = previousList.ToList();
                    list.Add(new SenderReceiverCallback<TSender>(header, handler.Target, null, handler));
                    newList = list;

                    result = true;
                }
            }
            while (!receiveCallbacks.TryUpdate(header, newList, previousList));

            return result;
        }

        public bool RemoveHandler(short header, Action<TSender, Packet> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            bool result;
            IReadOnlyList<ReceiveCallback<TSender>> previousList, newList;

            do
            {
                previousList = receiveCallbacks.GetOrAdd(header, ReceiverCallbackListFactory);

                var callback = previousList.FirstOrDefault(x => handler.Equals(x.Delegate));
                if (callback == null)
                {
                    newList = previousList;

                    result = false;
                }
                else
                {
                    var list = previousList.ToList();
                    list.Remove(callback);
                    newList = list;

                    result = true;
                }
            }
            while (!receiveCallbacks.TryUpdate(header, newList, previousList));

            return result;
        }
        #endregion

        #region - Listeners -
        /// <summary>
        /// Checks if the listener is attached to this message dispatcher.
        /// </summary>
        public bool IsListenerAttached(IListener receiver) => listeners.ContainsKey(receiver);

        /// <summary>
        /// Check if the message group for the specified listener is attached to this message dispatcher.
        /// </summary>
        public bool IsListenerAttached(IListener listener, object messageGroup)
        {
            if (!listeners.TryGetValue(listener, out IReadOnlyList<ListenerCallback> callbacks))
                return false;

            return callbacks.Any(callback => callback.Tags.Contains(messageGroup));
        }

        /// <summary>
        /// Attempts to attach the specified listener.
        /// Throws if any of the identifiers in the default message group failed to be resolved.
        /// </summary>
        /// <param name="listener">The listener to attach.</param>
        public bool AttachListener(IListener listener) => AttachListener(listener, out _);

        /// <summary>
        /// Attempts to attach the specified listener.
        /// Throws if any of the identifiers in the default message group failed to be resolved.
        /// </summary>
        /// <param name="listener">The listener to attach.</param>
        /// <param name="attachedGroups">The message groups that were successfully attached.</param>
        public bool AttachListener(IListener listener, out object[] attachedGroups)
        {
            attachedGroups = null;

            var listenerType = listener.GetType();
            var methodInfos = listenerType.FindAllMethods();

            var unresolvedIdentifiers = new Identifiers();
            var faultedGroups = new List<object>();

            /*
                Pass 1
                    Detect duplicate, unknown, invalid identifiers
                    Create a list of faulted groups
            */
            foreach (var methodInfo in methodInfos)
            {
                var identifiersAttributes = methodInfo.GetCustomAttributes<IdentifiersAttribute>();
                if (!identifiersAttributes.Any()) continue;

                object[] groups;
                var groupAttribute = methodInfo.GetCustomAttribute<GroupAttribute>();
                if (groupAttribute == null)
                    groups = new object[] { MessageGroups.Default };
                else
                    groups = groupAttribute.Tags;

                foreach (var identifiersAttribute in identifiersAttributes)
                {
                    var duplicateIdentifier = identifiersAttribute.Identifiers
                        .GroupBy(x => x)
                        .Where(x => x.Count() > 1)
                        .Select(x => x.Key)
                        .FirstOrDefault();

                    if (duplicateIdentifier != null)
                    {
                        throw new Exception(
                            $"Duplicate identifier '{duplicateIdentifier}' defined in " +
                            $"{identifiersAttribute.GetType().Name} for method " +
                            $"{listenerType.FullName}.{methodInfo.Name}"
                        );
                    }

                    foreach (var identifier in identifiersAttribute.Identifiers)
                    {
                        if (!Headers.HasIdentifier(identifier))
                            throw new UnknownIdentifierException(identifier, methodInfo);

                        if (!Headers.TryGetHeader(identifier, out short header) || header < 0)
                        {
                            unresolvedIdentifiers.Add(identifier);
                            foreach (var group in groups)
                            {
                                if (!faultedGroups.Contains(group))
                                    faultedGroups.Add(group);
                            }
                        }
                    }
                }
            }

            if (faultedGroups.Contains(MessageGroups.Default))
            {
                throw new ListenerAttachFailedException(listener, faultedGroups);
            }

            var callbackList = new List<ListenerCallback>();
            var successfulGroups = new List<object>();

            /*
                Pass 2
                    Generate receive/intercept callbacks for all non-faulted groups
            */
            foreach (var methodInfo in methodInfos)
            {
                object[] groups;
                var groupAttribute = methodInfo.GetCustomAttribute<GroupAttribute>();
                if (groupAttribute == null || !groupAttribute.Tags.Any())
                    groups = new object[] { MessageGroups.Default };
                else
                    groups = groupAttribute.Tags;

                // If all groups are faulted, skip this method
                if (groups.All(group => faultedGroups.Contains(group)))
                    continue;

                // Add group to successful list
                foreach (var group in groups)
                {
                    if (!faultedGroups.Contains(group) &&
                        !successfulGroups.Contains(group))
                    {
                        successfulGroups.Add(group);
                    }
                }

                // Receive
                var receiveAttribute = methodInfo.GetCustomAttribute<ReceiveAttribute>();
                if (receiveAttribute != null)
                {
                    if (!CheckReceiveMethodSignature(methodInfo))
                    {
                        throw new Exception(
                            $"{listenerType.Name}.{methodInfo.Name} has a " +
                            $"method signature incompatible with {receiveAttribute.GetType().Name}"
                        );
                    }

                    if (receiveAttribute.Identifiers.Count > 0)
                    {
                        var uniqueHeaders = new HashSet<short>();
                        foreach (var identifier in receiveAttribute.Identifiers)
                        {
                            short header = Headers[identifier];
                            if (!uniqueHeaders.Add(header)) continue;

                            callbackList.Add(CreateCallback(header, listener, groups, methodInfo));
                        }
                    }
                    else
                    {
                        callbackList.Add(CreateCallback(-1, listener, groups, methodInfo));
                    }
                }

                // Intercept
                var interceptAttributes = methodInfo.GetCustomAttributes<InterceptAttribute>();
                if (interceptAttributes.Count() > 1)
                    throw new Exception($"Multiple intercept attributes defined for method {listenerType.Name}.{methodInfo.Name}");

                var interceptAttribute = interceptAttributes.FirstOrDefault();
                if (interceptAttribute != null)
                {
                    if (!CheckInterceptorMethodSignature(methodInfo))
                    {
                        throw new Exception(
                            $"{listenerType.Name}.{methodInfo.Name} has a " +
                            $"method signature incompatible with {interceptAttribute.GetType().Name}"
                        );
                    }

                    if (interceptAttribute.Identifiers.Count > 0)
                    {
                        var uniqueHeaders = new HashSet<short>();
                        foreach (var identifier in interceptAttribute.Identifiers)
                        {
                            short header = Headers[identifier];
                            if (!uniqueHeaders.Add(header)) continue;

                            callbackList.Add(CreateInterceptCallback(identifier.Destination, header, listener, groups, methodInfo));
                        }
                    }
                    else
                    {
                        var destination = (interceptAttribute is InterceptOutAttribute) ? Destination.Server : Destination.Client;
                        callbackList.Add(CreateInterceptCallback(destination, -1, listener, groups, methodInfo));
                    }
                }
            }

            // If no listener callbacks and no intercept callbacks are left
            if (!callbackList.Any())
                return false;

            if (!listeners.TryAdd(listener, callbackList))
                throw new InvalidOperationException($"Listener '{listenerType.FullName}' is already attached");

            // Add receive callbacks
            foreach (var callbackGroup in callbackList.OfType<ReceiveCallback<TSender>>().GroupBy(x => x.Header))
            {
                short header = callbackGroup.Key;

                IReadOnlyList<ReceiveCallback<TSender>> previousList;
                List<ReceiveCallback<TSender>> updatedList;

                do
                {
                    previousList = receiveCallbacks.GetOrAdd(callbackGroup.Key, ReceiverCallbackListFactory);
                    updatedList = previousList.ToList();
                    updatedList.AddRange(callbackGroup);
                }
                while (!receiveCallbacks.TryUpdate(callbackGroup.Key, updatedList, previousList));
            }
            
            // Add intercept callbacks
            foreach (var callbackGroup in callbackList.OfType<InterceptCallback>().GroupBy(x => (x.Destination, x.Header)))
            {
                short header = callbackGroup.Key.Header;

                IReadOnlyList<InterceptCallback> previousList;
                List<InterceptCallback> updatedList;

                var map = callbackGroup.Key.Destination == Destination.Server ?
                    outgoingInterceptCallbacks : incomingInterceptCallbacks;

                do
                {
                    previousList = map.GetOrAdd(callbackGroup.Key.Header, InterceptCallbackListFactory);
                    updatedList = previousList.ToList();
                    updatedList.AddRange(callbackGroup);
                }
                while (!map.TryUpdate(callbackGroup.Key.Header, updatedList, previousList));
            }

            attachedGroups = successfulGroups.ToArray();
            return true;
        }

        public bool DetachListener(IListener listener)
        {
            if (!listeners.TryRemove(listener, out IReadOnlyList<ListenerCallback> callbacks))
                return false;

            foreach (var callback in callbacks)
                callback.Unsubscribe();

            // Receivers
            foreach (var callbackGroup in callbacks.OfType<ReceiveCallback<TSender>>().GroupBy(x => x.Header))
            {
                short header = callbackGroup.Key;

                IReadOnlyList<ReceiveCallback<TSender>> previousList;
                List<ReceiveCallback<TSender>> newList;

                do
                {
                    if (!receiveCallbacks.TryGetValue(header, out previousList))
                        break;

                    newList = previousList.ToList();
                    foreach (var callback in callbackGroup)
                        newList.Remove(callback);
                }
                while (!receiveCallbacks.TryUpdate(header, newList, previousList));
            }

            // Interceptors
            foreach (var callbackGroup in callbacks.OfType<InterceptCallback>().GroupBy(x => (x.Destination, x.Header)))
            {
                short header = callbackGroup.Key.Header;

                IReadOnlyList<InterceptCallback> previousList;
                List<InterceptCallback> newList;

                var map = callbackGroup.Key.Destination == Destination.Server ?
                    outgoingInterceptCallbacks : incomingInterceptCallbacks;

                do
                {
                    if (!map.TryGetValue(header, out previousList))
                        break;

                    newList = previousList.ToList();
                    foreach (var callback in callbackGroup)
                        newList.Remove(callback);
                }
                while (!map.TryUpdate(header, newList, previousList));
            }

            return true;
        }
        #endregion

        #region - Intercepts -
        public bool AddInterceptIn(short header, Action<InterceptEventArgs> callback)
            => AddIntercept(Destination.Client, header, callback);
        public bool AddInterceptOut(short header, Action<InterceptEventArgs> callback)
            => AddIntercept(Destination.Server, header, callback);
        public bool AddIntercept(Destination destination, short header, Action<InterceptEventArgs> callback)
        {
            bool result;
            IReadOnlyList<InterceptCallback> previousList, newList;
            var dict = GetInterceptCallbackDictionary(destination);

            do
            {
                previousList = dict.GetOrAdd(header, InterceptCallbackListFactory);

                if (previousList.Any(x => x.Delegate.Equals(callback)))
                {
                    newList = previousList;
                    result = false;
                }
                else
                {
                    var list = previousList.ToList();
                    list.Add(new ClosedInterceptCallback(destination, header, callback.Target, callback.Method, null, callback));
                    newList = list;
                    result = true;
                }
            }
            while (!dict.TryUpdate(header, newList, previousList));

            return result;
        }

        public bool RemoveInterceptIn(short header, Action<InterceptEventArgs> action)
            => RemoveIntercept(Destination.Client, header, action);
        public bool RemoveInterceptOut(short header, Action<InterceptEventArgs> action)
            => RemoveIntercept(Destination.Server, header, action);
        public bool RemoveIntercept(Destination destination, short header, Action<InterceptEventArgs> action)
        {
            bool result;
            IReadOnlyList<InterceptCallback> previousList, newList;
            var dict = GetInterceptCallbackDictionary(destination);

            do
            {
                previousList = dict.GetOrAdd(header, InterceptCallbackListFactory);

                var callback = previousList.FirstOrDefault(x => x.Delegate.Equals(action));
                if (callback != null)
                {
                    var list = previousList.ToList();
                    result = list.Remove(callback);
                    newList = list;
                }
                else
                {
                    newList = previousList;
                    result = false;
                }
            }
            while (!dict.TryUpdate(header, newList, previousList));

            return result;
        }
        #endregion
    }

    public class MessageDispatcher<TSender> : MessageDispatcher<Headers, IncomingHeaders, OutgoingHeaders, TSender>
    {
        public MessageDispatcher(Headers headers) : base(headers) { }
    }

    public class MessageDispatcher : MessageDispatcher<object>
    {
        public MessageDispatcher(Headers headers) : base(headers) { }
    }
}
