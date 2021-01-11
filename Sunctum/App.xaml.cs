

using NLog;
using Sunctum.Core;
using Sunctum.Views;
using System;
using System.Reflection;
using System.Windows;

namespace Sunctum
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// 参考：http://d.hatena.ne.jp/hilapon/20131203/1386046128
    /// </summary>
    public partial class App : Application
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        internal static SunctumBootstrapper Bootstrapper { get; private set; }

        /// <summary>
        /// アプリケーション開始時のイベントハンドラ
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            s_logger.Info($"Sunctum Personal Photo Library {version}");
            s_logger.Info("Copyright (C) dhq_boiler 2015-2021. All rights reserved.");
            s_logger.Info("SUNCTUM IS LAUNCHING");

            Bootstrapper = new SunctumBootstrapper();
            Bootstrapper.Run();
        }

        /// <summary>
        /// WPF UIスレッドでの未処理例外スロー時のイベントハンドラ
        /// </summary>
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            this.ReportUnhandledException(e.Exception);
        }

        /// <summary>
        /// UIスレッド以外の未処理例外スロー時のイベントハンドラ
        /// </summary>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.ReportUnhandledException(e.ExceptionObject as Exception);
        }

        private void ReportUnhandledException(Exception exception)
        {
            s_logger.Error(exception);
            var dialog = new ErrorReportDialog(exception);
            dialog.ShowDialog();
        }

        /// <summary>
        /// アプリケーション終了時のイベントハンドラ
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            s_logger.Info("SUNCTUM IS SHUTTING DOWN");

            this.DispatcherUnhandledException -= App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;

            base.OnExit(e);
        }
    }
}
