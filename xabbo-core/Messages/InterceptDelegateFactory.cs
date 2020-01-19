using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Xabbo.Core.Messages
{
    internal static class InterceptDelegateFactory
    {
        private static ConcurrentDictionary<MethodInfo, Delegate> cache = new ConcurrentDictionary<MethodInfo, Delegate>();

        public static Action<object, InterceptEventArgs> GetOpenDelegate(MethodInfo methodInfo)
        {
            if (!cache.TryGetValue(methodInfo, out Delegate del))
            {
                var generator = typeof(InterceptDelegateFactory)
                    .GetMethod(
                        "GenerateWeaklyTypedOpenDelegate",
                        BindingFlags.NonPublic | BindingFlags.Static
                    )
                    .MakeGenericMethod(methodInfo.DeclaringType);
                del = (Delegate)generator.Invoke(null, new object[] { methodInfo });
            }

            return (Action<object, InterceptEventArgs>)del;
        }

        private static Action<object, InterceptEventArgs> GenerateWeaklyTypedOpenDelegate<TTarget>(MethodInfo methodInfo)
        {
            var param = methodInfo.GetParameters();
            if (param.Length == 1 &&
                param[0].ParameterType.Equals(typeof(InterceptEventArgs)))
            {
                var call = (Action<TTarget, InterceptEventArgs>)methodInfo.CreateDelegate(typeof(Action<TTarget, InterceptEventArgs>));
                return new Action<object, InterceptEventArgs>((target, args) => call((TTarget)target, args));
            }

            throw new Exception($"Unable to generate delegate, method {methodInfo.DeclaringType.FullName}.{methodInfo.Name} has an invalid signature");
        }
    }
}
