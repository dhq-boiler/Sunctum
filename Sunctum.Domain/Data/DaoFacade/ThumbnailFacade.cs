

using Homura.ORM;
using NLog;
using Sunctum.Domain.Bridge;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunctum.Domain.Data.DaoFacade
{
    public static class ThumbnailFacade
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        internal static void Insert(ThumbnailViewModel target, DataOperationUnit dataOpUnit = null)
        {
            ThumbnailDao dao = new ThumbnailDao();
            dao.Insert(target.ToEntity(), dataOpUnit?.CurrentConnection);
            s_logger.Debug($"INSERT Thumbnail:{target}");
        }

        public static async Task DeleteWhereIDIs(Guid id, DataOperationUnit dataOpUnit = null)
        {
            ThumbnailDao dao = new ThumbnailDao();
            await dao.DeleteWhereIDIsAsync(id, dataOpUnit?.CurrentConnection).ConfigureAwait(false);
            s_logger.Debug($"DELETE Thumbnail:{id}");
        }

        public static async Task<ThumbnailViewModel> FindByImageID(Guid imageId, DataOperationUnit dataOpUnit = null)
        {
            ThumbnailDao dao = new ThumbnailDao();
            var items = (await dao.FindByAsync(new Dictionary<string, object>() { { "ImageID", imageId } }, dataOpUnit?.CurrentConnection).ToListAsync().ConfigureAwait(false)).SingleOrDefault();
            if (items == null)
            {
                return null;
            }
            return items.ToViewModel();
        }

        public static bool Exists(Guid imageId, DataOperationUnit dataOpUnit = null)
        {
            ThumbnailDao dao = new ThumbnailDao();
            return dao.CountBy(new Dictionary<string, object>() { { "ImageID", imageId } }, dataOpUnit?.CurrentConnection) > 0;
        }
        public static void Update(ThumbnailViewModel target, DataOperationUnit dataOpUnit = null)
        {
            ThumbnailDao dao = new ThumbnailDao();
            dao.Update(target.ToEntity(), dataOpUnit?.CurrentConnection);
            s_logger.Debug($"UPDATE Thumbnail:{target}");
        }

        internal static void InsertOrReplace(ThumbnailViewModel target, DataOperationUnit dataOpUnit = null)
        {
            ThumbnailDao dao = new ThumbnailDao();
            dao.InsertOrReplace(target.ToEntity(), dataOpUnit?.CurrentConnection);
            s_logger.Debug($"UPDATE Thumbnail:{target}");
        }
    }
}
