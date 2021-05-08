

using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.DaoFacade
{
    public static class StarFacade
    {
        public static void InsertOrReplace(BookViewModel target)
        {
            var dao = new StarDao();
            dao.InsertOrReplace(new Models.Star() { ID = target.ID, TypeId = 0, Level = target.StarLevel });
        }

        public static IEnumerable<BookViewModel> FindBookByStar(int? level)
        {
            var dao = new StarDao();
            return dao.FindBookByStar(level);
        }
    }
}
