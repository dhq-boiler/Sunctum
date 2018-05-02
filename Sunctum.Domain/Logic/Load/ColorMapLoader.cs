

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Data.Entity;
using Sunctum.Domain.Logic.BookSorting;
using Sunctum.Domain.Models;
using Sunctum.Infrastructure.Data.Rdbms;
using static Sunctum.Domain.Logic.BookSorting.CoverComparator;

namespace Sunctum.Domain.Logic.Load
{
    public static class ColorMapLoader
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static CoverSegmentCollection LoadColorMap(Guid bookID, Guid imageID, string path, CoverComparator.Color color)
        {
            var maps = ColorMapFacade.FindBy(bookID, imageID, (int)color);

            CoverSegmentCollection csc = new CoverSegmentCollection();
            if (maps != null && maps.Count() == Specifications.VERTICAL_SEGMENT_COUNT * Specifications.HORIZONTAL_SEGMENT_COUNT)
            {
                csc.AddRange(maps.OrderBy(x => x.ValueOrder).Select(x => new CoverSegmentColorInfo(x.Value)));
            }
            else
            {
                var colorInfoList = ComputeColorInfo(path, color);

                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => UpdateOrInsertColorMap(colorInfoList, bookID, imageID, color)));

                csc.AddRange(colorInfoList.Select(y => new CoverSegmentColorInfo(y.Median)));
            }
            return csc;
        }

        private static void UpdateOrInsertColorMap(IEnumerable<CoverSegmentColorInfo> colorInfoList, Guid bookID, Guid imageID, CoverComparator.Color color)
        {
            using (var dou = new DataOperationUnit())
            {
                dou.Open(ConnectionManager.DefaultConnection);

                for (int i = 0; i < colorInfoList.Count(); ++i)
                {
                    var value = colorInfoList.ElementAt(i);
                    ColorMapFacade.InsertOrReplace(new ColorMap(bookID, (int)color, i, value.Median, imageID, Specifications.HORIZONTAL_SEGMENT_COUNT, Specifications.VERTICAL_SEGMENT_COUNT), dou);
                }
            }
        }
    }
}
