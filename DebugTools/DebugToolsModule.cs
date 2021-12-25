using Prism.Ioc;
using Prism.Modularity;
using Sunctum.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugTools
{
    public class DebugToolsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
#if DEBUG
            containerRegistry.Register<IAddMenuPlugin, CopyAsYamlFormatPlugin>();
#endif
        }
    }
}
