

using Homura.ORM;
using NLog;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunctum.Domain.Data.DaoFacade
{
    public static class EncryptImageFacade
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static IEnumerable<EncryptImage> FindAll(DataOperationUnit dataOpUnit = null)
        {
            EncryptImageDao dao = new EncryptImageDao();
            return dao.FindAll(dataOpUnit?.CurrentConnection);
        }

        public static async IAsyncEnumerable<EncryptImage> FindAllAsync(DataOperationUnit dataOpUnit = null)
        {
            EncryptImageDao dao = new EncryptImageDao();
            var items = await dao.FindAllAsync(dataOpUnit?.CurrentConnection).ToListAsync();
            foreach (var item in items.AsParallel())
            {
                yield return item;
            }
        }

        public static EncryptImage FindBy(Guid targetImageId, DataOperationUnit dataOperationUnit = null)
        {
            EncryptImageDao dao = new EncryptImageDao();
            return dao.FindBy(new Dictionary<string, object>() { { "TargetImageID", targetImageId } }, dataOperationUnit?.CurrentConnection).SingleOrDefault();
        }

        public static async Task<EncryptImage> FindByAsync(Guid targetImageId, DataOperationUnit dataOperationUnit = null)
        {
            EncryptImageDao dao = new EncryptImageDao();
            return (await dao.FindByAsync(new Dictionary<string, object>() { { "TargetImageID", targetImageId } }, dataOperationUnit?.CurrentConnection).ToListAsync()).SingleOrDefault();
        }

        internal static async Task DeleteBy(Guid targetImageId, DataOperationUnit dataOperationUnit = null)
        {
            EncryptImageDao dao = new EncryptImageDao();
            await dao.DeleteAsync(new Dictionary<string, object>() { { "TargetImageID", targetImageId } }, dataOperationUnit?.CurrentConnection);
        }

        public static async Task<int> CountAllAsync(DataOperationUnit dataOpUnit = null)
        {
            EncryptImageDao dao = new EncryptImageDao();
            return await dao.CountAllAsync(dataOpUnit?.CurrentConnection);
        }

        internal static bool AnyEncrypted()
        {
            EncryptImageDao dao = new EncryptImageDao();
            return dao.FindAll().Any();
        }

        internal static async Task<bool> AnyEncryptedAsync()
        {
            EncryptImageDao dao = new EncryptImageDao();
            return await dao.FindAllAsync().AnyAsync();
        }
    }
}
