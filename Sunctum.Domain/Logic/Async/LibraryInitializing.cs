

using Homura.Core;
using Homura.ORM;
using Homura.ORM.Migration;
using Homura.ORM.Setup;
using Ninject;
using NLog;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Data.Dao.Migration.Plan;
using Sunctum.Domain.Logic.Parse;
using Sunctum.Domain.Models.Conversion;
using Sunctum.Domain.Models.Managers;
using System.Data.SQLite;
using System.Diagnostics;

namespace Sunctum.Domain.Logic.Async
{
    public class LibraryInitializing : AsyncTaskMakerBase, ILibraryInitializing
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        [Inject]
        public ILibrary LibraryManager { get; set; }

        [Inject]
        public IByteSizeCalculating ByteSizeCalculatingService { get; set; }

        [Inject]
        public IDirectoryNameParserManager DirectoryNameParserManager { get; set; }

        [Inject]
        public IBookTagInitializing BookTagInitializingService { get; set; }

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
            if (LibraryManager == null) return;
            ByteSizeCalculatingService.Range = ByteSizeCalculating.UpdateRange.IsAll;
            await LibraryManager.TaskManager.Enqueue(ByteSizeCalculatingService.GetTaskSequence());
        }

        private async void LibraryInitializing_FinishToUpgradeTo_Version_3(object sender, VersionChangeEventArgs e)
        {
            await LibraryManager.TaskManager.Enqueue(BookTagInitializingService.GetTaskSequence());
        }
    }
}
