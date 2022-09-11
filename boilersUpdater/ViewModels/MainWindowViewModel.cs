using ChinhDo.Transactions;
using Homura.Core;
using Homura.ORM;
using Homura.ORM.Setup;
using Newtonsoft.Json;
using NLog;
using NLog.LayoutRenderers.Wrappers;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Forms;
using boilersUpdater.Data.Dao;
using boilersUpdater.Data.Dao.Migration.Plan;
using boilersUpdater.Domain.Data.Dao;
using boilersUpdater.Models;
using System.Threading;
using System.Reactive.Linq;
using System.Windows.Input;

namespace boilersUpdater.ViewModels
{
    public enum EStage
    {
        /// <summary>
        /// アップデート承認待ち
        /// </summary>
        Stage1,

        /// <summary>
        /// Zipダウンロード&展開
        /// </summary>
        Stage2,

        /// <summary>
        /// アップデートログ確認待ち
        /// </summary>
        Stage3,

        /// <summary>
        /// アップデート完了
        /// </summary>
        Stage4,
    }

    public class MainWindowViewModel : BindableBase, IDisposable
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private bool disposedValue;

        private CompositeDisposable disposables = new CompositeDisposable();
        public ReactivePropertySlim<string> ProductName { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> FromVersion { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ToVersion { get; set; } = new ReactivePropertySlim<string>();
        public ReactiveCollection<string> Contents { get; set; } = new ReactiveCollection<string>();
        public ReactivePropertySlim<EStage> Stage { get; set; } = new ReactivePropertySlim<EStage>(EStage.Stage1);
        public ReactivePropertySlim<int> ProgressPercentage { get; set; } = new ReactivePropertySlim<int>(0);
        public ReactivePropertySlim<long> BytesReceived { get; set; } = new ReactivePropertySlim<long>(0);
        public ReactivePropertySlim<long> TotalBytesToReceive { get; set; } = new ReactivePropertySlim<long>(0);
        public ReactiveCollection<string> History { get; set; } = new ReactiveCollection<string>();

        public ReactiveCommand StartboilersUpdater { get; }
        public ReactiveCommand Cancel { get; }
        public ReactiveCommand Next { get; }
        private Latest Latest { get; set; }
        private Uri browser_download_url { get; set; }
        private WebClient WebClient { get; set; }
        public string downloadFileName { get; private set; }

        public MainWindowViewModel()
        {
            ConnectionManager.SetDefaultConnection($"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "vc.db")}", typeof(SQLiteConnection));

            DataVersionManager dvManager = new DataVersionManager();
            dvManager.CurrentConnection = ConnectionManager.DefaultConnection;
            dvManager.Mode = VersioningStrategy.ByTick;
            dvManager.RegisterChangePlan(new ChangePlan_VC_VersionOrigin());
            dvManager.UpgradeToTargetVersion();

            var dao = new VersionControlDao();
            var records = dao.FindAll();

            var target = records.Last();
            FromVersion.Value = $"v{target.Major}.{target.Minor}.{target.Build}.{target.Revision}";
            ToVersion.Value = RetrieveToVersionFromGithub().Result;

            Contents.Add($"{ProductName.Value} {FromVersion.Value} から");
            Contents.Add($"{ProductName.Value} {ToVersion.Value} にアップデートします。");
            Contents.Add("");
            Contents.Add("よろしければアップデートボタンを押してください。");
            Contents.Add("");

            Stage.Value = EStage.Stage1;

            StartboilersUpdater = new ReactiveCommand();
            StartboilersUpdater.Subscribe(() =>
            {
                browser_download_url = Latest.assets.Where(x => x.name.EndsWith(".zip"))
                                                    .Select(x => x.browser_download_url)
                                                    .First();
                var curDirPath = Directory.GetCurrentDirectory();
                var up1DirPath = curDirPath.Substring(0, curDirPath.LastIndexOf(@"\") + 1);
                var up1Dir = new DirectoryInfo(up1DirPath);
                ClearFilesAndDirectories();
                FullDirList(up1Dir, "*");
                files = files.Where(x => !x.FullName.StartsWith(curDirPath)).Where(x => !ExceptedList().Contains(x.FullName)).ToList();
                while (!files.All(file => !IsFileLocked(file)))
                {
                    var str = "アップデート対象のファイルが別のプロセスに開かれています。\n"
                            + "別のプロセスを終了してください。\n"
                            + "\n";
                    files.Select(file => new { File = file, IsLocked = IsFileLocked(file) })
                         .Where(x => x.IsLocked)
                         .ToList()
                         .ForEach(x => str += $"{x.File}\n");
                    var result = System.Windows.Forms.MessageBox.Show(str, "別のプロセスに開かれています", MessageBoxButtons.AbortRetryIgnore);
                    if (result == DialogResult.Abort)
                    {
                        Cancel.Execute();
                        return;
                    }
                    else if (result == DialogResult.Retry)
                    {
                        continue;
                    }
                    else if (result == DialogResult.Ignore)
                    {
                        break;
                    }
                }

                Stage.Value = EStage.Stage2;

                var guid = Guid.NewGuid();
                downloadFileName = $"C:\\Temp\\{guid.ToString("N")}.dat";
                using (var client = new WebClient())
                {
                    WebClient = client;
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36");
                    client.DownloadProgressChanged += Client_DownloadProgressChanged;
                    client.DownloadFileCompleted += Client_DownloadFileCompleted;
                    client.DownloadFileAsync(browser_download_url, downloadFileName);
                    History.Add($"ダウンロード中：{browser_download_url}");
                }
            }).AddTo(disposables);
            Cancel = new ReactiveCommand();
            Cancel.Subscribe(() =>
            {
                App.Current.Shutdown();
            }).AddTo(disposables);
            Next = Stage.Select(x => x.Equals(EStage.Stage3))
                        .ToReadOnlyReactivePropertySlim()
                        .ToReactiveCommand()
                        .WithSubscribe(() =>
                        {
                            Stage.Value = EStage.Stage4;
                            
                            var dao = new VersionControlDao();

                            using (DataOperationUnit dataOpUnit = new DataOperationUnit())
                            {
                                try
                                {
                                    dataOpUnit.Open(ConnectionManager.DefaultConnection);
                                    dataOpUnit.BeginTransaction();

                                    var records = dao.FindAll();
                                    if (records.Any())
                                    {
                                        var oldrecord = records.Last();
                                        oldrecord.RetiredDate = DateTime.Now;
                                        dao.Update(oldrecord, dataOpUnit.CurrentConnection);
                                    }

                                    var regex = new Regex("v(?<major>.+?)(.(?<minor>.+?)(.(?<build>.+?)(.(?<revision>.+?))?)?)?");
                                    var mc = regex.Match(Latest.tag_name);
                                    var newrecord = new VersionControl()
                                    {
                                        FullVersion = Latest.tag_name,
                                        Major = int.Parse(mc.Groups["major"].Value),
                                        Minor = mc.Groups.ContainsKey("minor") ? int.Parse(mc.Groups["minor"].Value) : 0,
                                        Build = mc.Groups.ContainsKey("build") ? int.Parse(mc.Groups["build"].Value) : 0,
                                        Revision = mc.Groups.ContainsKey("revision") ? int.Parse(mc.Groups["revision"].Value) : 0,
                                        IsValid = true,
                                        InstalledDate = DateTime.Now,
                                        RetiredDate = null,
                                    };
                                    dao.Insert(newrecord, dataOpUnit.CurrentConnection);

                                    dataOpUnit.Commit();
                                }
                                catch (Exception)
                                {
                                    dataOpUnit.Rollback();
                                }
                            }
                        })
                        .AddTo(disposables);
        }

        private void ClearFilesAndDirectories()
        {
            files.Clear();
            folders.Clear();
        }

        private IEnumerable<string> ExceptedList()
        {
            var curdir = Directory.GetCurrentDirectory();
            yield return Path.Combine(curdir, "Homura.dll");
            yield return Path.Combine(curdir, "Newtonsoft.Json.dll");
            yield return Path.Combine(curdir, "NLog.dll");
            yield return Path.Combine(curdir, "Prism.dll");
            yield return Path.Combine(curdir, "Prism.Unity.Wpf.dll");
            yield return Path.Combine(curdir, "Prism.Wpf.dll");
            yield return Path.Combine(curdir, "ReactiveProperty.Core.dll");
            yield return Path.Combine(curdir, "ReactiveProperty.dll");
            yield return Path.Combine(curdir, "System.Data.SQLite.dll");
            yield return Path.Combine(curdir, "System.Reactive.dll");
            yield return Path.Combine(curdir, "Unity.Abstractions.dll");
            yield return Path.Combine(curdir, "Unity.Container.dll");
            yield return Path.Combine(curdir, "boilersUpdater.dll");
            yield return Path.Combine(curdir, "boilersUpdater.pdb");
            yield return Path.Combine(curdir, "runtimes\\win\\lib\\net6.0\\System.Data.OleDb.dll");
        }

        private void Client_DownloadFileCompleted(object? sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            History.Add($"ダウンロード完了");

            var zipFile = ZipFile.Open(downloadFileName, ZipArchiveMode.Read);

            int totalCount = zipFile.Entries.Count();
            int currentCount = 0;

            ProgressPercentage.Value = currentCount / totalCount * 100;

            try
            {
                using (var scope = new TransactionScope())
                {
                    var txFileManager = new TxFileManager();
                    foreach (var entry in zipFile.Entries)
                    {
                        var isDirectory = entry.FullName.EndsWith("/") && entry.Name.Equals(string.Empty);
                        var entryFullName = entry.FullName.Replace("/", "\\");
                        var curDirPath = Directory.GetCurrentDirectory();
                        if (isDirectory)
                        {
                            var targetDirPath = Path.Combine(curDirPath, entryFullName);
                            History.Add($"ディレクトリ作成：{targetDirPath}");
                            txFileManager.CreateDirectory(targetDirPath);
                        }
                        else
                        {
                            var filename = Path.Combine(curDirPath, entryFullName);
                            txFileManager.Snapshot(filename);
                            History.Add($"ファイルコピー：{filename}");
                            entry.ExtractToFile(filename, true);
                        }

                        ProgressPercentage.Value = ++currentCount / totalCount * 100;
                    }

                    scope.Complete();

                    History.Add($"アップデート完了");

                    Stage.Value = EStage.Stage3;

                    Contents.Clear();
                    Contents.Add($"{ProductName.Value} {FromVersion.Value} から");
                    Contents.Add($"{ProductName.Value} {ToVersion.Value} にアップデート完了しました。");
                }
            }
            catch (Exception ex)
            {
                History.Add($"{History.Last()} でエラーが発生しました。{ex.ToString()}");
            }
            finally
            {
                zipFile.Dispose();
                //後処理
                if (File.Exists(downloadFileName))
                {
                    File.Delete(downloadFileName);
                }
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressPercentage.Value = e.ProgressPercentage;
            BytesReceived.Value += e.BytesReceived;
            TotalBytesToReceive.Value = e.TotalBytesToReceive;
        }

        private List<FileInfo> files = new List<FileInfo>();  // List that will hold the files and subfiles in path
        private List<DirectoryInfo> folders = new List<DirectoryInfo>(); // List that hold direcotries that cannot be accessed
        private void FullDirList(DirectoryInfo dir, string searchPattern)
        {
            // Console.WriteLine("Directory {0}", dir.FullName);
            // list the files
            try
            {
                foreach (FileInfo f in dir.GetFiles(searchPattern))
                {
                    //Console.WriteLine("File {0}", f.FullName);
                    files.Add(f);
                }
            }
            catch
            {
                s_logger.Error("Directory {0}  \n could not be accessed!!!!", dir.FullName);
                return;  // We alredy got an error trying to access dir so dont try to access it again
            }

            // process each directory
            // If I have been able to see the files in the directory I should also be able 
            // to look at its directories so I dont think I should place this in a try catch block
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                folders.Add(d);
                FullDirList(d, searchPattern);
            }

        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        private async Task<string> RetrieveToVersionFromGithub()
        {
            var dao = new GitHubReleasesLatestDao();
            var records = dao.FindAll();
            var record = records.Single();

            var client = new WebClient();
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36");
            var uri = new Uri(record.URL);
            string releases = client.DownloadString(uri);
            Latest = JsonConvert.DeserializeObject<Latest>(releases);
            var regex = new Regex("https://api.github.com/repos/(?<username>.+?)/(?<productname>.+?)/releases/latest");
            var match = regex.Match(record.URL);
            if (match.Success)
            {
                ProductName.Value = match.Groups["productname"].Value;
            }
            return Latest.tag_name;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    disposables.Dispose();
                }

                disposables = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
