﻿using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity;

namespace Sunctum.Domain.Logic.Async
{
    public class RecentOpenedLibraryUpdating : AsyncTaskMakerBase, IRecentOpenedLibraryUpdating
    {
        [Dependency]
        public Lazy<ILibrary> LibraryManager { get; set; }

        [Dependency]
        public IDataAccessManager DataAccessManager { get; set; }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            sequence.Add(async () =>
            {
                await LibraryManager.Value.AccessDispatcherObject(async () =>
                {
                    LibraryManager.Value.RecentOpenedLibraryList?.Clear();
                    LibraryManager.Value.RecentOpenedLibraryList = new ObservableCollection<RecentOpenedLibrary>();
                });

                var dao = DataAccessManager.AppDao.Build<RecentOpenedLibraryDao>();

                var hisotries = dao.FindAll();
                var matchOrNull = hisotries.Where(h => h.Path != null && h.Path.Equals(Configuration.ApplicationConfiguration.WorkingDirectory)).SingleOrDefault();
                if (matchOrNull != null)
                {
                    matchOrNull.AccessOrder = 0;
                    await dao.UpdateAsync(matchOrNull);
                }
                else
                {
                    var newhisotry = new RecentOpenedLibrary();
                    newhisotry.AccessOrder = 0;
                    newhisotry.Path = Configuration.ApplicationConfiguration.WorkingDirectory;
                    await dao.InsertAsync(newhisotry);
                }
                var other = hisotries.Where(h => h.Path != null && !h.Path.Equals(Configuration.ApplicationConfiguration.WorkingDirectory)).OrderBy(h => h.AccessOrder).ToArray();
                for (int i = 1; i < Math.Min(other.Count(), Configuration.ApplicationConfiguration.LibraryHistoryRecordCount) + 1; ++i)
                {
                    var o = other.ElementAt(i - 1);
                    o.AccessOrder = i;
                    await dao.UpdateAsync(o);
                    await LibraryManager.Value.AccessDispatcherObject(async () =>
                    {
                        LibraryManager.Value.RecentOpenedLibraryList.Add(o);
                    });
                }
                for (int i = Configuration.ApplicationConfiguration.LibraryHistoryRecordCount; i < other.Count() - Configuration.ApplicationConfiguration.LibraryHistoryRecordCount; ++i)
                {
                    var o = other.ElementAt(i);
                    await dao.DeleteAsync(new Dictionary<string, object>() { { "Path", o.Path } });
                }
            });
        }
    }
}
