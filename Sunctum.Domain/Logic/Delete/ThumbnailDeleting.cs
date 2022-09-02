

using Homura.ORM;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.ViewModels;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading;

namespace Sunctum.Domain.Logic.Delete
{
    public static class ThumbnailDeleting
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static void DeleteThumbnail(ImageViewModel target, DataOperationUnit dataOpUnit = null)
        {
            Contract.Requires(target != null);
            Contract.Requires(target.Thumbnail != null);

            var deleting = target.Thumbnail;
            if (deleting.RelativeMasterPath is null)
            {
                ThumbnailFacade.DeleteWhereIDIs(deleting.ID);
                return;
            }

            Thread.Sleep(0);

            File.Delete(deleting.AbsoluteMasterPath);
            s_logger.Debug($"Deleted File:{deleting.AbsoluteMasterPath}");

            ThumbnailFacade.DeleteWhereIDIs(deleting.ID);

            target.Thumbnail = null;
        }
    }
}
