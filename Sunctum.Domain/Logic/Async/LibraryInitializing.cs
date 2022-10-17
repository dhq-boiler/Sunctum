

using Homura.Core;
using Homura.ORM;
using Homura.ORM.Mapping;
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
using System.Linq;
using System.Threading.Tasks;
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
                    dvManager.RegisterChangePlan(new ChangePlan_VersionOrigin(VersioningMode.ByTick));
                    dvManager.RegisterChangePlan(new ChangePlan_Version_1(VersioningMode.ByTick));
                    dvManager.GetPlan(new Version_1()).FinishedToUpgradeTo += LibraryInitializing_FinishToUpgradeTo_Version_1;
                    dvManager.RegisterChangePlan(new ChangePlan_Version_2(VersioningMode.ByTick));
                    dvManager.RegisterChangePlan(new ChangePlanVersion3(VersioningMode.ByTick));
                    dvManager.GetPlan(new Version_3()).FinishedToUpgradeTo += LibraryInitializing_FinishToUpgradeTo_Version_3;
                    dvManager.RegisterChangePlan(new ChangePlan_Version_4(VersioningMode.ByTick));
                    dvManager.RegisterChangePlan(new ChangePlan_Version_5(VersioningMode.ByTick));
                    dvManager.GetPlan(new Version_5()).FinishedToUpgradeTo += LibraryInitializing_FinishedToUpgradeTo_Version_5;
                    dvManager.RegisterChangePlan(new ChangePlan_Version_6(VersioningMode.ByTick));
                    dvManager.GetPlan(new Version_6()).FinishedToUpgradeTo += LibraryInitializing_FinishedToUpgradeTo_Version_6;
                    dvManager.RegisterChangePlan(new ChangePlan_Version_7(VersioningMode.ByTick));
                    dvManager.GetPlan(new Version_7()).FinishedToUpgradeTo += LibraryInitializing_FinishedToUpgradeTo_Version_7;
                    dvManager.RegisterChangePlan(new ChangePlan_Version_8(VersioningMode.ByTick));
                    dvManager.RegisterChangePlan(new ChangePlan_Version_9(VersioningMode.ByTick));
                    dvManager.GetPlan(new Version_9()).FinishedToUpgradeTo += LibraryInitializing_FinishedToUpgradeTo_Version_9;
                    dvManager.FinishedToUpgradeTo += DvManager_FinishedToUpgradeTo;

                    dvManager.UpgradeToTargetVersion().GetAwaiter().GetResult();

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
                var dao = new KeyValueDao();
                var record = dao.FindBy(new System.Collections.Generic.Dictionary<string, object>() { { "Key", "LibraryID" } }).SingleOrDefault();
                var libraryId = record?.Value;
                if (libraryId is null)
                {
                    dao.Insert(new KeyValue()
                    {
                        Key = "LibraryID",
                        Value = Guid.NewGuid().ToString(),
                    });
                }
            });

            sequence.Add(() =>
            {
                DirectoryNameParserManager.Load();
            });
        }

        private async void LibraryInitializing_FinishedToUpgradeTo_Version_9(object sender, VersionChangeEventArgs e)
        {
            using (var conn = await ConnectionManager.DefaultConnection.OpenConnectionAsync())
            {
                using (var transaction = await conn.BeginTransactionAsync())
                {
                    long count = 0;
                    var dao = new EncryptImageDao();
                    await DirectQuery.RunQueryAsync(conn, async (conn) =>
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            using (var query = new Select().Count().From.Table(new Table<EncryptImage>()).Where.Column("EncryptFilePath").Like.Value(Configuration.ApplicationConfiguration.WorkingDirectory + "%"))
                            {
                                cmd.CommandText = query.ToSql();
                                cmd.CommandType = System.Data.CommandType.Text;
                                query.SetParameters(cmd);
                                count = (long)await cmd.ExecuteScalarAsync();
                            }
                        }
                    });

                    if (count == 0)
                    {
                        return;
                    }    

                    try
                    {
                        await DirectQuery.RunQueryAsync(conn, async (conn) =>
                        {
                            using (var cmd = conn.CreateCommand())
                            {
                                using (var query = new Update().Table(new Table<EncryptImage>()).Set.Column("EncryptFilePath").EqualTo.ReplaceColumn("EncryptFilePath", Configuration.ApplicationConfiguration.WorkingDirectory, String.Empty))
                                {
                                    cmd.CommandText = query.ToSql();
                                    cmd.CommandType = System.Data.CommandType.Text;
                                    await cmd.ExecuteNonQueryAsync();
                                }
                            }
                        });
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                    }
                }
            }
        }

        private void LibraryInitializing_FinishedToUpgradeTo_Version_7(object sender, VersionChangeEventArgs e)
        {
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
            await TaskManager.Enqueue(ByteSizeCalculatingService.GetTaskSequence()).ConfigureAwait(false);
        }

        private async void LibraryInitializing_FinishToUpgradeTo_Version_3(object sender, VersionChangeEventArgs e)
        {
            await TaskManager.Enqueue(BookTagInitializingService.GetTaskSequence()).ConfigureAwait(false);
        }

        private async void LibraryInitializing_FinishedToUpgradeTo_Version_5(object sender, VersionChangeEventArgs e)
        {
            BookHashingService.Range = BookHashing.UpdateRange.IsAll;
            await TaskManager.Enqueue(BookHashingService.GetTaskSequence()).ConfigureAwait(false);
        }

        private async void LibraryInitializing_FinishedToUpgradeTo_Version_6(object sender, VersionChangeEventArgs e)
        {
            using (var dataOpUnit = new DataOperationUnit())
            {
                await dataOpUnit.OpenAsync(ConnectionManager.DefaultConnection);
                await dataOpUnit.BeginTransactionAsync();

                var encryptImages = await EncryptImageFacade.FindAllAsync(dataOpUnit).ToListAsync();
                foreach (var encryptImage in encryptImages)
                {
                    var image = await ImageFacade.FindByAsync(encryptImage.TargetImageID, dataOpUnit);
                    image.IsEncrypted = true; //EncryptImageテーブルに存在するエントリは暗号化済みとしてマーク
                    await ImageFacade.UpdateAsync(image, dataOpUnit);
                }

                await dataOpUnit.CommitAsync();
            }
        }
    }
}
