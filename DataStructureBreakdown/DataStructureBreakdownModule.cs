using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Sunctum.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructureBreakdown
{
    public class DataStructureBreakdownModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
#if DEBUG
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ExtraBook", typeof(Views.DataStructureBreakdownMenu));
            regionManager.RegisterViewWithRegion("ExtraPage", typeof(Views.DataStructureBreakdownMenu));
            regionManager.RegisterViewWithRegion("ExtraTag", typeof(Views.DataStructureBreakdownMenu));
            regionManager.RegisterViewWithRegion("ExtraAuthor", typeof(Views.DataStructureBreakdownMenu));
#endif
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
#if DEBUG
            containerRegistry.Register<IAddMenuPlugin, DataStructureBreakdownPlugin>();
#endif
        }
    }
}
