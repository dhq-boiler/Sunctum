

using Homura.ORM;
using NLog;
using Sunctum.Domain.Bridge;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading;

namespace Sunctum.Domain.Data.DaoFacade
{
    public static class PageFacade
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static void Insert(PageViewModel target, DataOperationUnit dataOpUnit = null)
        {
            PageDao dao = new PageDao();
            dao.Insert(target.ToEntity(), dataOpUnit?.CurrentConnection);
            s_logger.Debug($"INSERT Page:{target}");
        }

        public static void DeleteWhereIDIs(Guid id, DataOperationUnit dataOpUnit = null)
        {
            PageDao dao = new PageDao();
            dao.DeleteWhereIDIs(id, dataOpUnit?.CurrentConnection);
            s_logger.Debug($"DELETE Page:{id}");
        }

        public static IEnumerable<PageViewModel> FindByBookId(Guid bookId, int challengeMaxCount = 3)
        {
            List<Exception> trying = new List<Exception>();
            while (trying.Count() < challengeMaxCount)
            {
                try
                {
                    PageDao dao = new PageDao();
                    return dao.FindBy(new Dictionary<string, object>() { { "BookID", bookId } }).ToViewModel();
                }
                catch (SQLiteException e)
                {
                    trying.Add(e);
                    Thread.Sleep(500);
                    continue;
                }
            }
            throw new QueryFailedException($"設定回数({challengeMaxCount})の問い合わせに失敗しました．", trying);
        }

        public static long CountAll()
        {
            var dao = new PageDao();
            return dao.CountAll();
        }

        public static PageViewModel FindByBookIdTop1(Guid bookId, DataOperationUnit dataOpUnit = null)
        {
            PageDao dao = new PageDao();
            return dao.FindByBookIdTop1(bookId, dataOpUnit?.CurrentConnection).FirstOrDefault().ToViewModel();
        }

        public static PageViewModel FindByImageId(Guid imageId)
        {
            var dao = new PageDao();
            return dao.FindBy(new Dictionary<string, object>() { { "ImageID", imageId } }).FirstOrDefault().ToViewModel(); ;
        }

        public static IEnumerable<PageViewModel> FindAll(DataOperationUnit dataOpUnit = null)
        {
            PageDao dao = new PageDao();
            return dao.FindAll().ToViewModel();
        }

        public static void IncrementPageIndex(Guid bookID, DataOperationUnit dataOpUnit = null)
        {
            PageDao dao = new PageDao();
            dao.IncrementPageIndex(bookID, dataOpUnit?.CurrentConnection);
            s_logger.Debug($"UPDATE Page bookId:{bookID}");
        }

        public static void Update(PageViewModel target, DataOperationUnit dataOpUnit = null)
        {
            PageDao dao = new PageDao();
            dao.Update(target.ToEntity(), dataOpUnit?.CurrentConnection);
            s_logger.Debug($"UPDATE Page:{target}");
        }

        public static void GetProperty(ref PageViewModel page, DataOperationUnit dataOpUnit = null)
        {
            var dao = new PageDao();
            dao.GetProperty(ref page, dataOpUnit?.CurrentConnection);
        }
    }
}
