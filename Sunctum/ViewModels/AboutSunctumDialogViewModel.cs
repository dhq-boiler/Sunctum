

using Prism.Commands;
using Prism.Mvvm;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Sunctum.ViewModels
{
    internal class AboutSunctumDialogViewModel : BindableBase
    {
        public string AssemblyVersion
        {
            get
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
                return versionInfo.FileVersion;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
                return versionInfo.LegalCopyright;
            }
        }

        public ICommand OkCommand { get; set; }

        public ICommand OpenBrowserCommand { get; set; }

        public AboutSunctumDialogViewModel()
        {
            OkCommand = new DelegateCommand<Window>((dialog) => dialog.DialogResult = true);
            OpenBrowserCommand = new DelegateCommand<string>(uri => Process.Start(new ProcessStartInfo(uri)));
        }
    }
}
