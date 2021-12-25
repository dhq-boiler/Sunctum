using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Sunctum.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderFilesImporter
{
    public class FolderFilesImporterModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDropPlugin, FolderFilesImporter>();
        }
    }
}
