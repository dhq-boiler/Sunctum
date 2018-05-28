

using Ninject;
using Prism.Mvvm;
using Reactive.Bindings;
using Sunctum.Domain.Models;
using Sunctum.Domain.ViewModels;
using Sunctum.UI.Controls;
using System;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Windows;

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

        [Inject]
        public IMainWindowViewModel MainWindowViewModel { get; set; }

        public ReactiveCommand<Window> OkCommand { get; set; } = new ReactiveCommand<Window>();

        public ReactiveCommand<Window> CancelCommand { get; set; } = new ReactiveCommand<Window>();

        public ReactiveCommand<TextBox> PathReferenceCommand { get; set; } = new ReactiveCommand<TextBox>();

        public PreferencesDialogViewModel()
        {
            var config = Configuration.ApplicationConfiguration;
            this.InitialConfig = config.ReadOnly();
            this.Config = (Configuration)config.Clone();

            RegisterCommands();
        }

        private void RegisterCommands()
        {
            OkCommand
                .Subscribe(dialog =>
                {
                    CheckUpdate(Config.WorkingDirectory, InitialConfig.WorkingDirectory);
                    CheckUpdate(Config.ConnectionString, InitialConfig.ConnectionString);
                    CheckUpdate(Config.LockFileInImporting, InitialConfig.LockFileInImporting);

                    CheckUpdate(Config.BookListViewItemAuthorHeight, InitialConfig.BookListViewItemAuthorHeight);
                    CheckUpdate(Config.BookListViewItemImageHeight, InitialConfig.BookListViewItemImageHeight);
                    CheckUpdate(Config.BookListViewItemTitleHeight, InitialConfig.BookListViewItemTitleHeight);
                    CheckUpdate(Config.BookListViewItemWidth, InitialConfig.BookListViewItemWidth);

                    CheckUpdate(Config.BookListViewItemMarginLeft, InitialConfig.BookListViewItemMarginLeft);
                    CheckUpdate(Config.BookListViewItemMarginTop, InitialConfig.BookListViewItemMarginTop);
                    CheckUpdate(Config.BookListViewItemMarginRight, InitialConfig.BookListViewItemMarginRight);
                    CheckUpdate(Config.BookListViewItemMarginBottom, InitialConfig.BookListViewItemMarginBottom);

                    CheckUpdate(Config.ContentListViewItemImageHeight, InitialConfig.ContentListViewItemImageHeight);
                    CheckUpdate(Config.ContentListViewItemTitleHeight, InitialConfig.ContentListViewItemTitleHeight);
                    CheckUpdate(Config.ContentListViewItemWidth, InitialConfig.ContentListViewItemWidth);

                    CheckUpdate(Config.ContentListViewItemMarginLeft, InitialConfig.ContentListViewItemMarginLeft);
                    CheckUpdate(Config.ContentListViewItemMarginTop, InitialConfig.ContentListViewItemMarginTop);
                    CheckUpdate(Config.ContentListViewItemMarginRight, InitialConfig.ContentListViewItemMarginRight);
                    CheckUpdate(Config.ContentListViewItemMarginBottom, InitialConfig.ContentListViewItemMarginBottom);

                    dialog.DialogResult = true;

                    bool willRestart = false;

                    if (RestartRequired)
                    {
                        willRestart = MessageBox.Show("変更を反映するには再起動が必要です.\n今すぐ再起動しますか？",
                        Process.GetCurrentProcess().MainWindowTitle, MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK;
                    }

                    if (!RestartRequired || willRestart)
                    {
                        Configuration.ApplicationConfiguration = Config;
                        Configuration.Save(Configuration.ApplicationConfiguration);
                    }

                    if (willRestart)
                    {
                        MainWindowViewModel.Close();
                        Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                    }
                });
            CancelCommand
                .Subscribe(dialog =>
                {
                    dialog.DialogResult = false;
                });
            PathReferenceCommand
                .Subscribe(textbox =>
                {
                    var dialog = new FolderSelectDialog();
                    dialog.InitialDirectory = textbox.Text;

                    if (dialog.ShowDialog() == true)
                    {
                        textbox.Text = dialog.FileName;
                    }
                });
        }

        public void CheckUpdate<T>(T left, T right)
        {
            if (!RestartRequired && !left.Equals(right))
            {
                RestartRequired = true;
            }
        }
    }
}
