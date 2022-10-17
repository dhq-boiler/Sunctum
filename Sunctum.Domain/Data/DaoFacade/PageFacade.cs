

using Homura.ORM;
using NLog;
using Sunctum.Domain.Bridge;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Logic.Encrypt;
using Sunctum.Domain.Models;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        public static async Task UpdateAsync(PageViewModel target, DataOperationUnit dataOpUnit = null)
        {
            string plainText = null;
            if (target.TitleIsEncrypted.Value && target.TitleIsDecrypted.Value)
            {
                plainText = target.Title;
                target.Title = await Encryptor.EncryptString(target.Title, Configuration.ApplicationConfiguration.Password);
                target.TitleIsDecrypted.Value = false;
            }
            PageDao dao = new PageDao();
            await dao.UpdateAsync(target.ToEntity(), dataOpUnit?.CurrentConnection);
            s_logger.Debug($"UPDATE Page:{target.ToString()}");
            if (target.TitleIsEncrypted.Value)
            {
                target.Title = await Encryptor.DecryptString(target.Title, Configuration.ApplicationConfiguration.Password);
                target.TitleIsDecrypted.Value = true;
            }
        }

        public static void GetProperty(ref PageViewModel page, DataOperationUnit dataOpUnit = null)
        {
            var dao = new PageDao();
            dao.GetProperty(ref page, dataOpUnit?.CurrentConnection);
        }
    }
}
