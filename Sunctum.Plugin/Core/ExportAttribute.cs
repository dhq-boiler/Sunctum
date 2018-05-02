

using System;

namespace Sunctum.Plugin.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExportAttribute : Attribute
    {
        public ExportAttribute(Type serviceInterface, ExportScope scope = ExportScope.Singleton)
        {
            ServiceInterface = serviceInterface;
            Scope = scope;
        }

        public Type ServiceInterface { get; private set; }

        public ExportScope Scope { get; private set; }
    }
}