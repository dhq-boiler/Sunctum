using CopyAsYamlFormat;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Sunctum.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyAsYamlFormat
{
    public class CopyAsYamlFormatModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
#if DEBUG
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ExtraBook", typeof(Views.CopyAsYamlFormatMenu));
            regionManager.RegisterViewWithRegion("ExtraPage", typeof(Views.CopyAsYamlFormatMenu));
            regionManager.RegisterViewWithRegion("ExtraTag", typeof(Views.CopyAsYamlFormatMenu));
            regionManager.RegisterViewWithRegion("ExtraAuthor", typeof(Views.CopyAsYamlFormatMenu));
#endif
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
#if DEBUG
            containerRegistry.Register<IAddMenuPlugin, CopyAsYamlFormatPlugin>();
#endif
        }
    }
}
