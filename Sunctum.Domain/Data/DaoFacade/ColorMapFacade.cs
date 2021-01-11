

using System;
using System.Collections.Generic;
using Homura.ORM;
using NLog;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Data.Entity;
using Sunctum.Domain.Models;

namespace Sunctum.Domain.Data.DaoFacade
{
    public static class ColorMapFacade
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static void InsertOrReplace(ColorMap target, DataOperationUnit dataOpUnit = null)
        {
            ColorMapDao dao = new ColorMapDao();
            dao.InsertOrReplace(target, dataOpUnit?.CurrentConnection);
            s_logger.Debug($"INSERT OR REPLACE ColorMap:{target}");
        }

        public static IEnumerable<ColorMap> FindBy(Guid bookID, Guid imageID, int channel, DataOperationUnit dataOpUnit = null)
        {
            ColorMapDao dao = new ColorMapDao();
            return dao.FindBy(new Dictionary<string, object>()
            {
                { "BookID", bookID },
                { "ImageID", imageID },
                { "Channel", channel },
                { "MaxX", Specifications.HORIZONTAL_SEGMENT_COUNT },
                { "MaxY", Specifications.VERTICAL_SEGMENT_COUNT }
            }, dataOpUnit?.CurrentConnection);
        }

        public static bool Exists(ColorMap entity, DataOperationUnit dataOpUnit = null)
        {
            ColorMapDao dao = new ColorMapDao();
            return dao.CountBy(new Dictionary<string, object>() { { "BookID", entity.BookID }, { "Channel", entity.Channel }, { "ValueOrder", entity.ValueOrder } }, dataOpUnit?.CurrentConnection) > 0;
        }
    }
}
