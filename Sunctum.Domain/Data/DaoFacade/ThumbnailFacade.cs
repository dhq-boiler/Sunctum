

using NLog;
using Sunctum.Domain.Bridge;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.ViewModels;
using Sunctum.Infrastructure.Data.Rdbms;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static void DeleteWhereIDIs(Guid id, DataOperationUnit dataOpUnit = null)
        {
            ThumbnailDao dao = new ThumbnailDao();
            dao.DeleteWhereIDIs(id, dataOpUnit?.CurrentConnection);
            s_logger.Debug($"DELETE Thumbnail:{id}");
        }

        public static ThumbnailViewModel FindByImageID(Guid imageId, DataOperationUnit dataOpUnit = null)
        {
            ThumbnailDao dao = new ThumbnailDao();
            var items = dao.FindBy(new Dictionary<string, object>() { { "ImageID", imageId } }, dataOpUnit?.CurrentConnection).SingleOrDefault();
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
