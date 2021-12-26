using Prism.Ioc;
using Prism.Modularity;
using Sunctum.Plugin;
using System;

namespace WpfLibrary1
{
    public class Class1 : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDropPlugin, Class2>();
        }
    }
}
