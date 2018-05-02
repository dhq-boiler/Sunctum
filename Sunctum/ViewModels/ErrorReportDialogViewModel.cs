

using NLog;
using Prism.Commands;
using Prism.Mvvm;
using Sunctum.Infrastructure.Data.Yaml;
using Sunctum.UI.Dialogs;
using System;
using System.Windows.Input;

namespace Sunctum.ViewModels
{
    internal class ErrorReportDialogViewModel : BindableBase
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        internal Exception OccuredException { get; set; }

        #region コマンド

        public ICommand ShowExceptionDetailsCommand { get; set; }

        public ICommand TerminateApplicationCommand { get; set; }

        #endregion //コマンド

        internal ErrorReportDialogViewModel(Exception exception)
        {
            OccuredException = exception;

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
                TerminateApplication();
            });
        }

        internal static void TerminateApplication()
        {
            s_logger.Info($"SUNCTUM IS SHUTTING DOWN");
            s_logger.Info($"BY ERROR REPORT DIALOG");
            Environment.Exit(0);
        }

        private void OpenExceptionDetailsDialog()
        {
            var ymlstr = YamlConverter.ExceptionToYaml(OccuredException);
            TreeViewDialog dialog = new TreeViewDialog(ymlstr);
            dialog.Title = $"Exception Details[{ OccuredException.GetType().Name}]";
            dialog.ShowDialog();
        }
    }
}
