using PickoutCover.ViewModels;
using PickoutCover.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickoutCover
{
    public class PickoutCoverModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ExtraPage", typeof(PickoutCoverMenu));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<Views.PickoutCover, PickoutCoverViewModel>();
            containerRegistry.RegisterSingleton<PickoutCoverViewModel>();
            containerRegistry.RegisterSingleton<ViewModels.PickoutCoverMenuViewModel>();
        }
    }
}
