

using NLog;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.Util;
using Sunctum.Infrastructure.Data.Rdbms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sunctum.Domain.Logic.Import
{
    internal class ImportIllust : ImportBook, IDisposable
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private List<string> _Paths;
        private Dictionary<string, FileStream> _fileStreams;

        internal ImportIllust(string[] paths)
        {
            _Paths = new List<string>(paths.OrderBy(a => a, new NaturalStringComparer()).ToArray());

            if (Configuration.ApplicationConfiguration.LockFileInImporting)
            {
                _fileStreams = new Dictionary<string, FileStream>();
                foreach (var path in paths)
                {
                    _fileStreams.Add(path, new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
                    s_logger.Debug($"Lock:{path}");
                }
            }
        }

        public override string ToString()
        {
            return "Import File(Illust) from " + Path;
        }

        public override void Estimate()
        {
            var targets = _Paths.Where(a => !File.GetAttributes(a).HasFlag(FileAttributes.Hidden) && Specifications.SupportedImageType.Any(b => a.EndsWith(b)));

            _children = new List<Importer>();
            foreach (var target in targets)
            {
                _children.Add(new ImportPage(target, true));
            }
            Count = _children.Count();
        }

        public override IEnumerable<System.Threading.Tasks.Task> GenerateTasks(ILibraryManager library, string copyTo, string entryName, DataOperationUnit dataOpUnit)
        {
            List<System.Threading.Tasks.Task> ret = new List<System.Threading.Tasks.Task>();

            var directoryPath = copyTo + "\\" + entryName;

            ret.Add(new System.Threading.Tasks.Task(() => CreateTaskToCreateDirectoryIfDoesntExist(directoryPath)));

            ret.Add(new System.Threading.Tasks.Task(() => CreateTaskToInsertBook(entryName, Title, dataOpUnit)));
            Processed = 0;

            var firstChild = _children.First();
            ProcessChild(library, dataOpUnit, ret, directoryPath, firstChild);
            ret.Add(new System.Threading.Tasks.Task(() => GenerateDeliverables(dataOpUnit)));
            ret.Add(new System.Threading.Tasks.Task(() => SetDeliverables(library)));

            for (int i = 1; i < _children.Count(); ++i)
            {
                var child = _children[i];
                ProcessChild(library, dataOpUnit, ret, directoryPath, child);
            }
            ret.Add(new System.Threading.Tasks.Task(() => WriteFilesSize()));
            ret.Add(new System.Threading.Tasks.Task(() => SwitchContentsRegisteredToTrue()));
            ret.Add(new System.Threading.Tasks.Task(() => Dispose()));
            ret.Add(new System.Threading.Tasks.Task(() => Log()));
            return ret;
        }

        private void ProcessChild(ILibraryManager library, DataOperationUnit dataOpUnit, List<System.Threading.Tasks.Task> ret, string directoryPath, Importer child)
        {
            if (child is ImportPage)
            {
                ImportPage x = child as ImportPage;
                x.PageIndex = Processed + 1;
                x.TotalPageCount = _children.Count();
                x.PageTitle = child.Name;
            }
            var tasks = child.GenerateTasks(library, directoryPath, System.IO.Path.GetFileNameWithoutExtension(child.Path), dataOpUnit);
            ret.AddRange(tasks);
            ++Processed;
        }

        private void Log()
        {
            s_logger.Info(Title);
        }

        #region IDisposable Support
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                }

                if (_fileStreams != null)
                {
                    foreach (var fs in _fileStreams)
                    {
                        string path = fs.Value.Name;
                        fs.Value.Dispose();
                        s_logger.Debug($"Unlock:{path}");
                    }
                    _fileStreams.Clear();
                    _fileStreams = null;
                }

                _disposedValue = true;
            }
        }

        ~ImportIllust()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
