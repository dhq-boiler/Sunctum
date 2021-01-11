

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

        public static IEnumerable<EncryptImage> FindAll()
        {
            EncryptImageDao dao = new EncryptImageDao();
            return dao.FindAll();
        }

        internal static EncryptImage FindBy(Guid targetImageId)
        {
            EncryptImageDao dao = new EncryptImageDao();
            return dao.FindBy(new Dictionary<string, object>() { { "TargetImageID", targetImageId } }).SingleOrDefault();
        }

        internal static void DeleteBy(Guid targetImageId)
        {
            EncryptImageDao dao = new EncryptImageDao();
            dao.Delete(new Dictionary<string, object>() { { "TargetImageID", targetImageId } });
        }

        internal static bool AnyEncrypted()
        {
            EncryptImageDao dao = new EncryptImageDao();
            return dao.FindAll().Any();
        }
    }
}
