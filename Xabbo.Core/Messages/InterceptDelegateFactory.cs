using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Xabbo.Core.Messages
{
    internal static class InterceptDelegateFactory
    {
        private static ConcurrentDictionary<MethodInfo, Delegate> cache = new ConcurrentDictionary<MethodInfo, Delegate>();

        public static Action<object, InterceptArgs> GetOpenDelegate(MethodInfo methodInfo)
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

            return (Action<object, InterceptArgs>)del;
        }

        private static Action<object, InterceptArgs> GenerateWeaklyTypedOpenDelegate<TTarget>(MethodInfo methodInfo)
        {
            var param = methodInfo.GetParameters();
            if (param.Length == 1 &&
                param[0].ParameterType.Equals(typeof(InterceptArgs)))
            {
                var call = (Action<TTarget, InterceptArgs>)methodInfo.CreateDelegate(typeof(Action<TTarget, InterceptArgs>));
                return new Action<object, InterceptArgs>((target, args) => call((TTarget)target, args));
            }

            throw new Exception($"Unable to generate delegate, method {methodInfo.DeclaringType.FullName}.{methodInfo.Name} has an invalid signature");
        }
    }
}
