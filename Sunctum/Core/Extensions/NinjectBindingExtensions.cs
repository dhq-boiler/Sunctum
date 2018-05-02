

using Ninject.Syntax;
using Sunctum.Plugin.Core;
using System.Reflection;
using static Ninject.Extensions.Conventions.ExtensionsForIKernel;

namespace Sunctum.Core.Extensions
{
    /*
     * Ninject equivalent to MEF AssemblyCatalog https://stackoverflow.com/questions/27578361/ninject-equivalent-to-mef-assemblycatalog
     * quizzer Frank https://stackoverflow.com/users/4317569/frank
     * answerer Frank https://stackoverflow.com/users/4317569/frank
     */

    internal static class NinjectBindingExtensions
    {
        public static void BindExportsInAssembly(this IBindingRoot root, Assembly assembly)
        {
            root.Bind(c => c.From(assembly)
                            .IncludingNonPublicTypes()
                            .SelectAllClasses()
                            .WithAttribute<ExportAttribute>()
                            .BindWith<ExportAttributeBindingGenerator>());
        }
    }
}
