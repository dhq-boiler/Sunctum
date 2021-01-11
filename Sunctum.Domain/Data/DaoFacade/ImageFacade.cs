

using Homura.ORM;
using NLog;
using Sunctum.Domain.Bridge;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Data.Rdbms.SQLite;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading;

namespace Sunctum.Domain.Data.DaoFacade
{
    public static class ImageFacade
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static void Insert(ImageViewModel target, DataOperationUnit dataOpUnit = null)
        {
            ImageDao dao = new ImageDao();
            dao.Insert(target.ToEntity(), dataOpUnit?.CurrentConnection);
            s_logger.Debug($"INSERT Image:{target}");
        }

        public static void DeleteWhereIDIs(Guid id, DataOperationUnit dataOpUnit = null)
        {
            ImageDao dao = new ImageDao();
            dao.DeleteWhereIDIs(id, dataOpUnit?.CurrentConnection);
            s_logger.Debug($"DELETE Image:{id}");
        }

        public static ImageViewModel FindBy(Guid id, DataOperationUnit dataOpUnit = null, int challengeMaxCount = 3)
        {
            List<Exception> trying = new List<Exception>();
            while (trying.Count() < challengeMaxCount)
            {
                try
                {
                    ImageDao dao = new ImageDao();
                    return dao.FindBy(new Dictionary<string, object>() { { "ID", id } }, dataOpUnit?.CurrentConnection).SingleOrDefault().ToViewModel();
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

        public static IEnumerable<ImageViewModel> FindAll(DataOperationUnit dataOpUnit = null)
        {
            ImageDao dao = new ImageDao();
            return dao.FindAll(dataOpUnit?.CurrentConnection).ToViewModel();
        }

        public static IEnumerable<ImageViewModel> GetAllImages(IEnumerable<EntryViewModel> entries, DataOperationUnit dataOpUnit = null)
        {
            List<ImageViewModel> ret = new List<ImageViewModel>();
            Queue<EntryViewModel> targetPool = new Queue<EntryViewModel>(entries);

            while (targetPool.Count() > 0)
            {
                List<Guid> bookIds = new List<Guid>();
                List<Guid> pageIds = new List<Guid>();
                List<Guid> imageIds = new List<Guid>();
                int count = 0;

                var entry = targetPool.Dequeue();

                var book = entry as BookViewModel;
                var page = entry as PageViewModel;
                var image = entry as ImageViewModel;

                if (book != null)
                {
                    bookIds.Add(book.ID);
                    ++count;
                }
                if (page != null)
                {
                    pageIds.Add(page.ID);
                    ++count;
                }
                if (image != null)
                {
                    imageIds.Add(image.ID);
                    ++count;
                }

                if (targetPool.Count() == 0 || count == SpecificationDefaults.SQLITE_MAX_VARIABLE_NUMBER)
                {
                    try
                    {
                        ImageDao dao = new ImageDao();
                        ret.AddRange(dao.GetAllImages(bookIds, pageIds, imageIds, dataOpUnit?.CurrentConnection).ToViewModel());
                    }
                    catch (SQLiteException)
                    {
                        throw;
                    }
                }
            }

            return ret;
        }

        public static void Update(ImageViewModel image)
        {
            ImageDao dao = new ImageDao();
            dao.Update(image.ToEntity());
        }
    }
}
