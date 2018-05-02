

using Ninject.Extensions.Conventions.BindingGenerators;
using Ninject.Syntax;
using Sunctum.Plugin.Core;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sunctum.Core
{
    /*
     * Ninject equivalent to MEF AssemblyCatalog https://stackoverflow.com/questions/27578361/ninject-equivalent-to-mef-assemblycatalog
     * quizzer Frank https://stackoverflow.com/users/4317569/frank
     * answerer Frank https://stackoverflow.com/users/4317569/frank
     */

    public class ExportAttributeBindingGenerator : IBindingGenerator
    {
        public IEnumerable<IBindingWhenInNamedWithOrOnSyntax<object>> CreateBindings(Type type, IBindingRoot bindingRoot)
        {
            var attribute = type.GetCustomAttribute<ExportAttribute>();
            var serviceType = attribute.ServiceInterface;

            if (!serviceType.IsAssignableFrom(type))
            {
                throw new Exception($"Error in ExportAttribute: Cannot bind type '{serviceType}' to type '{type}'.");
            }

            var binding = bindingRoot.Bind(serviceType).To(type);

            switch (attribute.Scope)
            {
                case ExportScope.Singleton:
                    yield return (IBindingWhenInNamedWithOrOnSyntax<object>)binding.InSingletonScope();
                    break;
                case ExportScope.Transient:
                    yield return (IBindingWhenInNamedWithOrOnSyntax<object>)binding.InTransientScope();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
