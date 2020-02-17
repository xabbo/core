using System;
using System.Collections.Generic;
using System.Linq;

namespace Xabbo.Core.Components
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DependenciesAttribute : Attribute
    {
        public IReadOnlyCollection<Type> Types { get; }

        public DependenciesAttribute(params Type[] dependencies)
        {
            foreach (var type in dependencies)
            {
                if (!type.IsSubclassOf(typeof(XabboComponent)))
                    throw new ArgumentException($"The dependency type {type.FullName} must be a subclass of XabboComponent");
            }

            Types = dependencies.ToList().AsReadOnly();
        }
    }
}
