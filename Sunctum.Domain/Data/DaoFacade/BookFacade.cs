

using Homura.ORM;
using NLog;
using Sunctum.Domain.Bridge;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Logic.Encrypt;
using Sunctum.Domain.Models;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunctum.Domain.Data.DaoFacade
{
    public static class BookFacade
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static async Task Insert(BookViewModel target, DataOperationUnit dataOpUnit = null)
        {
            BookDao dao = new BookDao();
            await dao.InsertAsync(target.ToEntity(), dataOpUnit?.CurrentConnection);
            s_logger.Debug($"INSERT Book:{target}");
        }

        public static async Task DeleteWhereIDIs(Guid id, DataOperationUnit dataOpUnit = null)
        {
            BookDao dao = new BookDao();
            await dao.DeleteWhereIDIsAsync(id, dataOpUnit?.CurrentConnection);
            s_logger.Debug($"DELETE Book:{id}");
        }

        public static IEnumerable<BookViewModel> FindByteSizeIsNull()
        {
            BookDao dao = new BookDao();
            return dao.FindBy(new Dictionary<string, object>() { { "ByteSize", null } }).ToViewModel();
        }

        public static IEnumerable<BookViewModel> FindHashIsNull()
        {
            var dao = new BookDao();
            return dao.FindBy(new Dictionary<string, object>() { { "FingerPrint", null } }).ToViewModel();
        }

        public static async IAsyncEnumerable<BookViewModel> FindAll(DataOperationUnit dataOpUnit = null)
        {
            BookDao dao = new BookDao();
            var items = await dao.FindAllAsync(dataOpUnit?.CurrentConnection).ToListAsync();
            foreach (var item in items)
            {
                yield return item.ToViewModel();
            }
        }

        public static async Task Update(BookViewModel book, DataOperationUnit dataOpUnit = null)
        {
            string plainText = null;
            if (book.TitleIsEncrypted.Value && book.TitleIsDecrypted.Value)
            {
                plainText = book.Title;
                book.Title = await Encryptor.EncryptString(book.Title, Configuration.ApplicationConfiguration.Password);
                book.TitleIsDecrypted.Value = false;
            }
            BookDao dao = new BookDao();
            await dao.UpdateAsync(book.ToEntity(), dataOpUnit?.CurrentConnection);
            s_logger.Debug($"UPDATE Book:{book}");
            if (book.TitleIsEncrypted.Value && !book.TitleIsDecrypted.Value)
            {
                book.TitleIsDecrypted.Value = true;
                book.Title = await Encryptor.DecryptString(book.Title, Configuration.ApplicationConfiguration.Password);
            }
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

        public static IEnumerable<BookViewModel> FindDuplicateFingerPrint()
        {
            var dao = new BookDao();
            return dao.FindDuplicateFingerPrint();
        }

        public static IEnumerable<BookViewModel> FindAllWithFillContents(DataOperationUnit dataOpUnit = null)
        {
            var dao = new BookDao();
            return dao.FindAllWithFillContents(dataOpUnit?.CurrentConnection);
        }

        public static async IAsyncEnumerable<BookViewModel> FindAllWithFillContentsAsync(DataOperationUnit dataOpUnit = null)
        {
            var dao = new BookDao();
            var items = await dao.FindAllWithFillContentsAsync(dataOpUnit?.CurrentConnection).ToListAsync();
            foreach (var item in items)
            {
                yield return item;
            }
        }

        public static void FillContents(ref BookViewModel book, DataOperationUnit dataOpUnit = null)
        {
            var dao = new BookDao();
            dao.FillContents(ref book, dataOpUnit?.CurrentConnection);
        }
    }
}
