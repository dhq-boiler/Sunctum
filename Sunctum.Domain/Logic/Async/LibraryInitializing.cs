

using Homura.Core;
using Homura.ORM;
using Homura.ORM.Migration;
using Homura.ORM.Setup;
using Homura.QueryBuilder.Iso.Dml;
using NLog;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Data.Dao.Migration.Plan;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Parse;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Conversion;
using Sunctum.Domain.Models.Managers;
using System;
using System.Data.SQLite;
using System.Diagnostics;
using Unity;

namespace Sunctum.Domain.Logic.Async
{
    public class LibraryInitializing : AsyncTaskMakerBase, ILibraryInitializing
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        [Dependency]
        public ITaskManager TaskManager { get; set; }

        [Dependency]
        public IByteSizeCalculating ByteSizeCalculatingService { get; set; }

        [Dependency]
        public IDirectoryNameParserManager DirectoryNameParserManager { get; set; }

        [Dependency]
        public IBookTagInitializing BookTagInitializingService { get; set; }

        [Dependency]
        public IBookHashing BookHashingService { get; set; }

        public Stopwatch Stopwatch { get; private set; } = new Stopwatch();

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start LibraryInitializing"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info("Checking Database schema..."));

            sequence.Add(() =>
            {
                try
                {
                    Stopwatch.Start();

                    System.Environment.SetEnvironmentVariable("TMP", "C:\\Temp");
                    System.Environment.SetEnvironmentVariable("TEMP", "C:\\Temp");

                    DataVersionManager dvManager = new DataVersionManager();
                    dvManager.CurrentConnection = ConnectionManager.DefaultConnection;
                    dvManager.Mode = VersioningStrategy.ByTick;
                    dvManager.RegisterChangePlan(new ChangePlan_VersionOrigin());
                    dvManager.RegisterChangePlan(new ChangePlan_Version_1());
                    dvManager.GetPlan(new Version_1()).FinishedToUpgradeTo += LibraryInitializing_FinishToUpgradeTo_Version_1;
                    dvManager.RegisterChangePlan(new ChangePlan_Version_2());
                    dvManager.RegisterChangePlan(new ChangePlan_Version_3());
                    dvManager.GetPlan(new Version_3()).FinishedToUpgradeTo += LibraryInitializing_FinishToUpgradeTo_Version_3;
                    dvManager.RegisterChangePlan(new ChangePlan_Version_4());
                    dvManager.RegisterChangePlan(new ChangePlan_Version_5());
                    dvManager.GetPlan(new Version_5()).FinishedToUpgradeTo += LibraryInitializing_FinishedToUpgradeTo_Version_5;
                    dvManager.RegisterChangePlan(new ChangePlan_Version_6());
                    dvManager.GetPlan(new Version_6()).FinishedToUpgradeTo += LibraryInitializing_FinishedToUpgradeTo_Version_6;
                    dvManager.RegisterChangePlan(new ChangePlan_Version_7());
                    dvManager.GetPlan(new Version_7()).FinishedToUpgradeTo += LibraryInitializing_FinishedToUpgradeTo_Version_7;
                    dvManager.FinishedToUpgradeTo += DvManager_FinishedToUpgradeTo;

                    dvManager.UpgradeToTargetVersion();

                    Stopwatch.Stop();

                    s_logger.Info($"Completed to check Database schema. {Stopwatch.ElapsedMilliseconds}ms");
                }
                catch (FailedOpeningDatabaseException)
                {
                    throw;
                }
                catch (SQLiteException e)
                {
                    s_logger.Error("Failed to check Database schema.");
                    s_logger.Debug(e);
                    throw;
                }
                finally
                {
                    if (Stopwatch.IsRunning)
                    {
                        Stopwatch.Stop();
                    }
                }
            });

            sequence.Add(() =>
            {
                DirectoryNameParserManager.Load();
            });
        }

        private void LibraryInitializing_FinishedToUpgradeTo_Version_7(object sender, VersionChangeEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish LibraryInitializing"));
        }

        private static void DvManager_FinishedToUpgradeTo(object sender, ModifiedEventArgs e)
        {
            s_logger.Info($"Heavy Modifying DB Count : {e.ModifiedCount}");

            if (e.ModifiedCount > 0)
            {
                SQLiteBaseDao<Dummy>.Vacuum(ConnectionManager.DefaultConnection);
            }
        }

        private async void LibraryInitializing_FinishToUpgradeTo_Version_1(object sender, VersionChangeEventArgs e)
        {
            ByteSizeCalculatingService.Range = ByteSizeCalculating.UpdateRange.IsAll;
            await TaskManager.Enqueue(ByteSizeCalculatingService.GetTaskSequence());
        }

        private async void LibraryInitializing_FinishToUpgradeTo_Version_3(object sender, VersionChangeEventArgs e)
        {
            await TaskManager.Enqueue(BookTagInitializingService.GetTaskSequence());
        }

        private async void LibraryInitializing_FinishedToUpgradeTo_Version_5(object sender, VersionChangeEventArgs e)
        {
            BookHashingService.Range = BookHashing.UpdateRange.IsAll;
            await TaskManager.Enqueue(BookHashingService.GetTaskSequence());
        }

        private void LibraryInitializing_FinishedToUpgradeTo_Version_6(object sender, VersionChangeEventArgs e)
        {
            using (var dataOpUnit = new DataOperationUnit())
            {
                dataOpUnit.Open(ConnectionManager.DefaultConnection);
                dataOpUnit.BeginTransaction();

                var encryptImages = EncryptImageFacade.FindAll(dataOpUnit);
                foreach (var encryptImage in encryptImages)
                {
                    var image = ImageFacade.FindBy(encryptImage.TargetImageID, dataOpUnit);
                    image.IsEncrypted = true; //EncryptImageテーブルに存在するエントリは暗号化済みとしてマーク
                    ImageFacade.Update(image, dataOpUnit);
                }

                dataOpUnit.Commit();
            }
        }
    }
}
