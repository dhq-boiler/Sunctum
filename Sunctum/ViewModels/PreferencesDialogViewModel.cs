

using Prism.Mvvm;
using Sunctum.Domain.Models;

namespace Sunctum.ViewModels
{
    public class PreferencesDialogViewModel : BindableBase
    {
        private Configuration _Config;

        private ReadOnlyConfiguration InitialConfig { get; set; }

        public Configuration Config
        {
            get { return _Config; }
            set { SetProperty(ref _Config, value); }
        }

        public bool RestartRequired { get; private set; }

        public PreferencesDialogViewModel()
        {
            var config = Configuration.ApplicationConfiguration;
            this.InitialConfig = config.ReadOnly();
            this.Config = (Configuration)config.Clone();
        }

        public void CheckUpdate_WorkingDirectory()
        {
            if (!RestartRequired && !Config.WorkingDirectory.Equals(InitialConfig.WorkingDirectory))
            {
                RestartRequired = true;
            }
        }

        public void CheckUpdate_LockFileInImporting()
        {
            if (!RestartRequired && Config.LockFileInImporting != InitialConfig.LockFileInImporting)
            {
                RestartRequired = true;
            }
        }

        public void CheckUpdate_ConnectionString()
        {
            if (!RestartRequired && Config.ConnectionString != InitialConfig.ConnectionString)
            {
                RestartRequired = true;
            }
        }

        public void CheckUpdate_BookListViewItemWidth()
        {
            if (!RestartRequired && Config.BookListViewItemWidth != InitialConfig.BookListViewItemWidth)
            {
                RestartRequired = true;
            }
        }

        public void CheckUpdate_BookListViewItemImageHeight()
        {
            if (!RestartRequired && Config.BookListViewItemImageHeight != InitialConfig.BookListViewItemImageHeight)
            {
                RestartRequired = true;
            }
        }

        public void CheckUpdate_BookListViewItemAuthorHeight()
        {
            if (!RestartRequired && Config.BookListViewItemAuthorHeight != InitialConfig.BookListViewItemAuthorHeight)
            {
                RestartRequired = true;
            }
        }

        public void CheckUpdate_BookListViewItemTitleHeight()
        {
            if (!RestartRequired && Config.BookListViewItemTitleHeight != InitialConfig.BookListViewItemTitleHeight)
            {
                RestartRequired = true;
            }
        }

        public void CheckUpdate_BookListViewItemMarginLeft()
        {
            if (!RestartRequired && Config.BookListViewItemMarginLeft != InitialConfig.BookListViewItemMarginLeft)
            {
                RestartRequired = true;
            }
        }

        public void CheckUpdate_BookListViewItemMarginTop()
        {
            if (!RestartRequired && Config.BookListViewItemMarginTop != InitialConfig.BookListViewItemMarginTop)
            {
                RestartRequired = true;
            }
        }

        public void CheckUpdate_BookListViewItemMarginRight()
        {
            if (!RestartRequired && Config.BookListViewItemMarginRight != InitialConfig.BookListViewItemMarginRight)
            {
                RestartRequired = true;
            }
        }

        public void CheckUpdate_BookListViewItemMarginBottom()
        {
            if (!RestartRequired && Config.BookListViewItemMarginBottom != InitialConfig.BookListViewItemMarginBottom)
            {
                RestartRequired = true;
            }
        }

        public void CheckUpdate_ContentListViewItemWidth()
        {
            if (!RestartRequired && Config.ContentListViewItemWidth != InitialConfig.ContentListViewItemWidth)
            {
                RestartRequired = true;
            }
        }

        public void CheckUpdate_ContentListViewItemImageHeight()
        {
            if (!RestartRequired && Config.ContentListViewItemImageHeight != InitialConfig.ContentListViewItemImageHeight)
            {
                RestartRequired = true;
            }
        }

        public void CheckUpdate_ContentListViewItemTitleHeight()
        {
            if (!RestartRequired && Config.ContentListViewItemTitleHeight != InitialConfig.ContentListViewItemTitleHeight)
            {
                RestartRequired = true;
            }
        }

        public void CheckUpdate_ContentListViewItemMarginLeft()
        {
            if (!RestartRequired && Config.ContentListViewItemMarginLeft != InitialConfig.ContentListViewItemMarginLeft)
            {
                RestartRequired = true;
            }
        }

        public void CheckUpdate_ContentListViewItemMarginTop()
        {
            if (!RestartRequired && Config.ContentListViewItemMarginTop != InitialConfig.ContentListViewItemMarginTop)
            {
                RestartRequired = true;
            }
        }

        public void CheckUpdate_ContentListViewItemMarginRight()
        {
            if (!RestartRequired && Config.ContentListViewItemMarginRight != InitialConfig.ContentListViewItemMarginRight)
            {
                RestartRequired = true;
            }
        }

        public void CheckUpdate_ContentListViewItemMarginBottom()
        {
            if (!RestartRequired && Config.ContentListViewItemMarginBottom != InitialConfig.ContentListViewItemMarginBottom)
            {
                RestartRequired = true;
            }
        }
    }
}
