

using Homura.ORM;
using NLog;
using Sunctum.Domain.Bridge;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.DaoFacade
{
    public static class BookFacade
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static void Insert(BookViewModel target, DataOperationUnit dataOpUnit = null)
        {
            BookDao dao = new BookDao();
            dao.Insert(target.ToEntity(), dataOpUnit?.CurrentConnection);
            s_logger.Debug($"INSERT Book:{target}");
        }

        public static void DeleteWhereIDIs(Guid id, DataOperationUnit dataOpUnit = null)
        {
            BookDao dao = new BookDao();
            dao.DeleteWhereIDIs(id, dataOpUnit?.CurrentConnection);
            s_logger.Debug($"DELETE Book:{id}");
        }

        public static IEnumerable<BookViewModel> FindByteSizeIsNull()
        {
            BookDao dao = new BookDao();
            return dao.FindBy(new Dictionary<string, object>() { { "ByteSize", DaoConst.Is.Null } }).ToViewModel();
        }

        public static IEnumerable<BookViewModel> FindAll(DataOperationUnit dataOpUnit = null)
        {
            BookDao dao = new BookDao();
            return dao.FindAll(dataOpUnit?.CurrentConnection).ToViewModel();
        }

        public static void Update(BookViewModel book)
        {
            BookDao dao = new BookDao();
            dao.Update(book.ToEntity());
            s_logger.Debug($"UPDATE Book:{book}");
        }

        internal static IEnumerable<BookViewModel> FindAllWithAuthor(DataOperationUnit dataOpUnit)
        {
            BookDao dao = new BookDao();
            return dao.FindAllWithAuthor(dataOpUnit?.CurrentConnection);
        }

        public static IEnumerable<BookViewModel> FindByAuthorId(Guid authorId)
        {
            BookDao dao = new BookDao();
            return dao.FindBy(new Dictionary<string, object>() { { "AuthorID", authorId } }).ToViewModel();
        }

        public static void GetProeprty(ref BookViewModel book, DataOperationUnit dataOpUnit = null)
        {
            BookDao dao = new BookDao();
            dao.GetProperty(ref book, dataOpUnit?.CurrentConnection);
        }
    }
}
