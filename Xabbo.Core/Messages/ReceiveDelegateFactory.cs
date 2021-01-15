using System;
using System.Collections.Concurrent;
using System.Reflection;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Messages
{
    internal static class ReceiveDelegateFactory
    {
        private static readonly ConcurrentDictionary<MethodInfo, Delegate> cache;

        static ReceiveDelegateFactory()
        {
            cache = new ConcurrentDictionary<MethodInfo, Delegate>();
        }

        public static Action<object, object, IReadOnlyPacket> GetOpenDelegate(MethodInfo method)
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

            return (Action<object, object, IReadOnlyPacket>)del;
        }

        private static Action<object, object, IReadOnlyPacket> GenerateWeaklyTypedOpenDelegate<TTarget>(MethodInfo method)
        {
            var param = method.GetParameters();
            if (param.Length == 0)
            {
                var call = (Action<TTarget>)method.CreateDelegate(typeof(Action<TTarget>));
                return new Action<object, object, IReadOnlyPacket>((target, sender, packet) => call((TTarget)target));
            }
            else if (param.Length == 1)
            {
                if (param[0].ParameterType == typeof(object))
                {
                    var call = (Action<TTarget, object>)method.CreateDelegate(typeof(Action<TTarget, object>));
                    return new Action<object, object, IReadOnlyPacket>((target, sender, packet) => call((TTarget)target, sender));
                }
                else if (param[0].ParameterType.IsAssignableFrom(typeof(IReadOnlyPacket)))
                {
                    var call = (Action<TTarget, IReadOnlyPacket>)method.CreateDelegate(typeof(Action<TTarget, IReadOnlyPacket>));
                    return new Action<object, object, IReadOnlyPacket>((target, sender, packet) => call((TTarget)target, packet));
                }
            }
            else if (param.Length == 2)
            {
                if (param[0].ParameterType == typeof(object) &&
                    param[1].ParameterType.IsAssignableFrom(typeof(IReadOnlyPacket)))
                {
                    var call = (Action<TTarget, object, IReadOnlyPacket>)method.CreateDelegate(typeof(Action<TTarget, object, IReadOnlyPacket>));
                    return new Action<object, object, IReadOnlyPacket>((target, sender, packet) => call((TTarget)target, sender, packet));
                }
            }

            throw new Exception($"Unable to generate delegate, method {method.DeclaringType.FullName}.{method.Name} has an invalid signature");
        }
    }
}
