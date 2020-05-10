using System;
using System.Collections.Concurrent;
using System.Reflection;

using Xabbo.Core.Protocol;

namespace Xabbo.Core.Messages
{
    internal static class ReceiveDelegateFactory
    {
        private static readonly ConcurrentDictionary<MethodInfo, Delegate> cache;

        static ReceiveDelegateFactory()
        {
            cache = new ConcurrentDictionary<MethodInfo, Delegate>();
        }

        public static Action<object, object, Packet> GetOpenDelegate(MethodInfo method)
        {
            if (!cache.TryGetValue(method, out Delegate del))
            {
                var generator =
                    typeof(ReceiveDelegateFactory)
                    .GetMethod(
                        "GenerateWeaklyTypedOpenDelegate",
                        BindingFlags.NonPublic | BindingFlags.Static
                    )
                    .MakeGenericMethod(method.DeclaringType);

                del = (Delegate)generator.Invoke(null, new object[] { method });
                cache.TryAdd(method, del);
            }

            return (Action<object, object, Packet>)del;
        }

        private static Action<object, object, Packet> GenerateWeaklyTypedOpenDelegate<TTarget>(MethodInfo method)
        {
            var param = method.GetParameters();
            if (param.Length == 0)
            {
                var call = (Action<TTarget>)method.CreateDelegate(typeof(Action<TTarget>));
                return new Action<object, object, Packet>((target, sender, packet) => call((TTarget)target));
            }
            else if (param.Length == 1)
            {
                if (param[0].ParameterType == typeof(object))
                {
                    var call = (Action<TTarget, object>)method.CreateDelegate(typeof(Action<TTarget, object>));
                    return new Action<object, object, Packet>((target, sender, packet) => call((TTarget)target, sender));
                }
                else if (param[0].ParameterType.IsAssignableFrom(typeof(Packet)))
                {
                    var call = (Action<TTarget, Packet>)method.CreateDelegate(typeof(Action<TTarget, Packet>));
                    return new Action<object, object, Packet>((target, sender, packet) => call((TTarget)target, packet));
                }
            }
            else if (param.Length == 2)
            {
                if (param[0].ParameterType == typeof(object) &&
                    param[1].ParameterType.IsAssignableFrom(typeof(Packet)))
                {
                    var call = (Action<TTarget, object, Packet>)method.CreateDelegate(typeof(Action<TTarget, object, Packet>));
                    return new Action<object, object, Packet>((target, sender, packet) => call((TTarget)target, sender, packet));
                }
            }

            throw new Exception($"Unable to generate delegate, method {method.DeclaringType.FullName}.{method.Name} has an invalid signature");
        }
    }
}
