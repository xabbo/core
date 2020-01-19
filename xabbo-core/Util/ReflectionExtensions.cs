using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xabbo.Core
{
    internal static class ReflectionExtensions
    {
        public static IEnumerable<MethodInfo> FindAllMethods(this Type type)
            => FindAllMethods(type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        private static IEnumerable<MethodInfo> FindAllMethods(Type type, BindingFlags flags)
        {
            IEnumerable<MethodInfo> methods = type.GetMethods(flags);

            if (type.BaseType != null)
            {
                methods = methods.Concat(FindAllMethods(type.BaseType, flags & ~BindingFlags.Public));
            }

            return methods;
        }
    }
}
