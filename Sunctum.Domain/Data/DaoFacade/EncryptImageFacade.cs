

using Homura.ORM;
using NLog;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        internal static EncryptImage FindBy(Guid targetImageId, DataOperationUnit dataOperationUnit = null)
        {
            EncryptImageDao dao = new EncryptImageDao();
            return dao.FindBy(new Dictionary<string, object>() { { "TargetImageID", targetImageId } }, dataOperationUnit?.CurrentConnection).SingleOrDefault();
        }

        internal static void DeleteBy(Guid targetImageId, DataOperationUnit dataOperationUnit = null)
        {
            EncryptImageDao dao = new EncryptImageDao();
            dao.Delete(new Dictionary<string, object>() { { "TargetImageID", targetImageId } }, dataOperationUnit?.CurrentConnection);
        }

        internal static bool AnyEncrypted()
        {
            EncryptImageDao dao = new EncryptImageDao();
            return dao.FindAll().Any();
        }
    }
}
