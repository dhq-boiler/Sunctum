

using Ninject;
using NLog;
using Sunctum.Domain.Extensions;
using Sunctum.Domain.Logic.Import;
using Sunctum.Domain.Logic.Parse;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sunctum.Domain.Logic.Async
{
    public class BookImporting : AsyncTaskMakerBase, IBookImporting
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public BookImporting()
        { }

        [Inject]
        public ILibrary LibraryManager { get; set; }

        [Inject]
        public IDirectoryNameParserManager DirectoryNameParserManager { get; set; }

        public string MasterDirectory { get; set; }

        public IEnumerable<string> ObjectPaths { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start BookImporting"));
            sequence.Add(() => s_logger.Info($"      MasterDirectory '{MasterDirectory}'"));
            sequence.Add(() => s_logger.Info($"      ObjectPaths '{ObjectPaths.ArrayToString()}'"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            List<Importer> importers = new List<Importer>();

            DiscriminateDroppedEntries(ObjectPaths, importers, LibraryManager);

            var workingDirectory = Configuration.ApplicationConfiguration.WorkingDirectory;
            var now = DateTime.Now;
            var copyTo = workingDirectory + "\\"
                + MasterDirectory + "\\"
                + now.Year + "\\"
                + now.Month.ToString("00") + "\\"
                + now.Day.ToString("00");

            //保存先ディレクトリ準備
            if (!Directory.Exists(copyTo))
            {
                Directory.CreateDirectory(copyTo);
                s_logger.Debug($"Create directory:{copyTo}");
            }

            List<Importer> willNotImport = new List<Importer>();

            //構造把握+全体数計上
            foreach (var task in importers)
            {
                task.Estimate();
                if (task.Count == 0)
                {
                    willNotImport.Add(task);
                }
            }

            //メタデータ分析
            foreach (var task in importers)
            {
                if (task is ImportIllust)
                {
                    var importer = task as ImportIllust;
                    importer.Title = DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                else if (task is ImportBook)
                {
                    var importer = task as ImportBook;

                    string sourceDirName = ((File.GetAttributes(importer.Path) & FileAttributes.Directory) == FileAttributes.Directory)
                                            ? Path.GetFileName(importer.Path)
                                            : Path.GetDirectoryName(importer.Path);

                    var dirNameParser = DirectoryNameParserManager.Get(sourceDirName);
                    dirNameParser.Parse(sourceDirName);

                    if (dirNameParser.HasTags)
                    {
                        importer.TagNames = dirNameParser.Tags;
                    }

                    if (dirNameParser.HasAuthor)
                    {
                        importer.AuthorName = dirNameParser.Author;
                    }

                    importer.Title = dirNameParser.Title;
                }
            }

            //画像0件のインポート単位はインポートしない
            foreach (var task in willNotImport)
            {
                importers.Remove(task);
                s_logger.Warn($"{task.Path} will not be imported.");
            }

            sequence.Add(new System.Threading.Tasks.Task(() => s_logger.Info($"Began to import.")));
            for (int i = 0; i < importers.Count(); ++i)
            {
                var task = importers.ElementAt(i);
                Guid entryNameSeedGuid = Guid.NewGuid();
                var entryName = entryNameSeedGuid.ToString("N");
                var t = task.GenerateTasks(LibraryManager, copyTo, entryName, null);
                sequence.AddRange(t);
            }
            sequence.Add(new System.Threading.Tasks.Task(() => s_logger.Info($"Completed to import.")));
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish BookImporting"));
        }

        private static void DiscriminateDroppedEntries(IEnumerable<string> objectPaths, List<Importer> importers, ILibrary library)
        {
            s_logger.Info($"Start Dropped entries discrimination");

            foreach (var objectPath in objectPaths.Where(p => Directory.Exists(p)))
            {
                importers.Add(Importer.CreateInstance(objectPath, library));
            }

            var files = objectPaths.Where(p => File.Exists(p));
            if (files.Count() > 0)
            {
                importers.Add(Importer.CreateInstance(files.ToArray()));
            }

            foreach (var task in importers)
            {
                s_logger.Info($"Discriminated : {task}");
            }

            s_logger.Info($"Finish Dropped entries discrimination");
        }
    }
}
