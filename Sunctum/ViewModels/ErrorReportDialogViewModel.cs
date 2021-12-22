

using NLog;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Sunctum.Infrastructure.Data.Yaml;
using Sunctum.Properties;
using Sunctum.UI.Dialogs;
using System;
using System.Windows.Input;

namespace Sunctum.ViewModels
{
    internal class ErrorReportDialogViewModel : BindableBase, IDialogAware
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public event Action<IDialogResult> RequestClose;

        public Exception OccuredException { get; set; }

        #region コマンド

        public ICommand ShowExceptionDetailsCommand { get; set; }

        public ICommand TerminateApplicationCommand { get; set; }

        public string Title => Resources.ErrorReportDialogTitle;

        #endregion //コマンド

        public ErrorReportDialogViewModel()
        {
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            ShowExceptionDetailsCommand = new DelegateCommand(() =>
            {
                OpenExceptionDetailsDialog();
            });
            TerminateApplicationCommand = new DelegateCommand(() =>
            {
                RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
                TerminateApplication();
            });
        }

        internal static void TerminateApplication()
        {
            s_logger.Info($"SUNCTUM IS SHUTTING DOWN");
            s_logger.Info($"BY ERROR REPORT DIALOG");
            Environment.Exit(1);
        }

        private void OpenExceptionDetailsDialog()
        {
            var ymlstr = YamlConverter.ExceptionToYaml(OccuredException);
            TreeViewDialog dialog = new TreeViewDialog(ymlstr);
            dialog.Title = $"Exception Details[{ OccuredException.GetType().Name}]";
            dialog.ShowDialog();
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            OccuredException = parameters.GetValue<Exception>("Exception");
        }
    }
}
