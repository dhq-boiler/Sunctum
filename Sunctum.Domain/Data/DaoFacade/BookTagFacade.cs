

using NLog;
using Sunctum.Domain.Bridge;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.DaoFacade
{
    public static class BookTagFacade
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static void Insert(BookTagViewModel target)
        {
            var dao = new BookTagDao();
            dao.Insert(target.ToEntity());
            s_logger.Debug($"INSERT BookTag:{target}");
        }

        public static bool Exists(BookTagViewModel target)
        {
            var dao = new BookTagDao();
            return dao.CountBy(new Dictionary<string, object>() { { "BookID", target.BookID }, { "TagID", target.TagID } }) > 0;
        }

        public static void Delete(BookTagViewModel deleteEntity)
        {
            var dao = new BookTagDao();
            dao.Delete(new Dictionary<string, object>() { { "BookID", deleteEntity.BookID }, { "TagID", deleteEntity.TagID } });
        }

        public static IEnumerable<BookTagViewModel> FindAll()
        {
            var dao = new BookTagDao();
            return dao.FindAll().ToViewModel();
        }
    }
}
