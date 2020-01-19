using System;
using System.Collections.Concurrent;
using System.Reflection;

using Xabbo.Core.Protocol;

namespace Xabbo.Core.Messages
{
    internal static class ReceiveDelegateFactory<TSender>
    {
        private static readonly ConcurrentDictionary<MethodInfo, Delegate> cache;

        static ReceiveDelegateFactory()
        {
            cache = new ConcurrentDictionary<MethodInfo, Delegate>();
        }

        public static Action<object, TSender, Packet> GetOpenDelegate(MethodInfo method)
        {
            if (!cache.TryGetValue(method, out Delegate del))
            {
                var generator =
                    typeof(ReceiveDelegateFactory<TSender>)
                    .GetMethod(
                        "GenerateWeaklyTypedOpenDelegate",
                        BindingFlags.NonPublic | BindingFlags.Static
                    )
                    .MakeGenericMethod(method.DeclaringType);

                del = (Delegate)generator.Invoke(null, new object[] { method });
                cache.TryAdd(method, del);
            }

            return (Action<object, TSender, Packet>)del;
        }

        private static Action<object, TSender, Packet> GenerateWeaklyTypedOpenDelegate<TTarget>(MethodInfo method)
        {
            var param = method.GetParameters();
            if (param.Length == 0)
            {
                var call = (Action<TTarget>)method.CreateDelegate(typeof(Action<TTarget>));
                return new Action<object, TSender, Packet>((target, sender, packet) => call((TTarget)target));
            }
            else if (param.Length == 1)
            {
                if (param[0].ParameterType.IsAssignableFrom(typeof(TSender)))
                {
                    var call = (Action<TTarget, TSender>)method.CreateDelegate(typeof(Action<TTarget, TSender>));
                    return new Action<object, TSender, Packet>((target, sender, packet) => call((TTarget)target, sender));
                }
                else if (param[0].ParameterType.IsAssignableFrom(typeof(Packet)))
                {
                    var call = (Action<TTarget, Packet>)method.CreateDelegate(typeof(Action<TTarget, Packet>));
                    return new Action<object, TSender, Packet>((target, sender, packet) => call((TTarget)target, packet));
                }
            }
            else if (param.Length == 2)
            {
                if (param[0].ParameterType.IsAssignableFrom(typeof(TSender)) &&
                    param[1].ParameterType.IsAssignableFrom(typeof(Packet)))
                {
                    var call = (Action<TTarget, TSender, Packet>)method.CreateDelegate(typeof(Action<TTarget, TSender, Packet>));
                    return new Action<object, TSender, Packet>((target, sender, packet) => call((TTarget)target, sender, packet));
                }
            }

            throw new Exception($"Unable to generate delegate, method {method.DeclaringType.FullName}.{method.Name} has an invalid signature");
        }
    }
}
