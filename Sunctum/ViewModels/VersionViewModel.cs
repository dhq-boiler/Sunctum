﻿using NLog;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Sunctum.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Disposables;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sunctum.ViewModels
{
    public class VersionViewModel : BindableBase, IDialogAware
    {
        private CompositeDisposable _disposables = new CompositeDisposable();
        public string Title => "Sunctum のバージョン情報";

        public event Action<IDialogResult> RequestClose;

        public ReactivePropertySlim<string> Version { get; } = new ReactivePropertySlim<string>();

        public ReactivePropertySlim<string> License { get; } = new ReactivePropertySlim<string>();

        public ReactivePropertySlim<string> Markdown { get; } = new ReactivePropertySlim<string>();

        public ReactiveCommand OKCommand { get; } = new ReactiveCommand();

        public VersionViewModel()
        {
            Version.Value = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            License.Value = LicenseReadToEnd();
            //Markdown.Value = LicenseMdReadToEnd();
            OKCommand.Subscribe(() =>
            {
                DialogResult result = new DialogResult(ButtonResult.OK);
                RequestClose.Invoke(result);
            })
            .AddTo(_disposables);
        }

        private string LicenseReadToEnd()
        {
            const string filename = "LICENSE";
            var str = ReadFileToEnd(filename);
            if (!string.IsNullOrEmpty(str))
                return str;

            using (var client = new WebClient())
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                var flags = ParseProductVersion(versionInfo.ProductVersion);
                if (flags.isPreRelease)
                {
                    var url = "https://raw.githubusercontent.com/dhq-boiler/Sunctum/develop/LICENSE?token=AB4M2N3WFDFDPRCRRNX2E63BYGJHW";
                    DownloadFile(client, url, filename);
                }
                else if (flags.isMaster)
                {
                    var url = "https://raw.githubusercontent.com/dhq-boiler/Sunctum/master/LICENSE?token=AB4M2N62Q2CCRX7H2QBEUGDBYGJKU";
                    DownloadFile(client, url, filename);
                }
            }

            if (File.Exists(filename))
            {
                str = ReadFileToEnd(filename);
                if (!string.IsNullOrEmpty(str))
                    return str;
            }

            str = $"{filename}が見つかりません。";
            MessageBox.Show(str);
            return str;
        }

        private (bool isMaster, bool isPreRelease, bool isDebug) ParseProductVersion(string productVersion)
        {
            if (productVersion.Contains("master"))
                return (true, false, false);
            if (productVersion.Contains("unstable"))
                return (false, true, false);
            if (productVersion.Contains("feature"))
                return (false, false, true);
            throw new UnexpectedException("ProductVersionをパースできませんでした。");
        }

        private static void DownloadFile(WebClient client, string url, string file)
        {
            client.DownloadFile(url, file);
            LogManager.GetCurrentClassLogger().Info($"Download {file} from {url}");
        }

        private string ReadFileToEnd(string filename)
        {
            try
            {
                using (var streamReader = new StreamReader(new FileStream(filename, FileMode.Open)))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (FileNotFoundException ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex);
                return string.Empty;
            }
        }

        private string LicenseMdReadToEnd()
        {
            const string filename = "LICENSE.md";
            var str = ReadFileToEnd(filename);
            if (!string.IsNullOrEmpty(str))
                return str;

            using (var client = new WebClient())
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                var flags = ParseProductVersion(versionInfo.ProductVersion);
                if (flags.isPreRelease)
                {
                    var url = "https://raw.githubusercontent.com/dhq-boiler/boiler-s-Graphics/develop/boilersGraphics/LICENSE.md";
                    DownloadFile(client, url, filename);
                }
                else if (flags.isMaster)
                {
                    var url = "https://raw.githubusercontent.com/dhq-boiler/boiler-s-Graphics/master/boilersGraphics/LICENSE.md";
                    DownloadFile(client, url, filename);
                }
            }

            if (File.Exists(filename))
            {
                str = ReadFileToEnd(filename);
                if (!string.IsNullOrEmpty(str))
                    return str;
            }

            str = $"{filename}が見つかりません。";
            MessageBox.Show(str);
            return str;
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
        }
    }
}
