

using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.ViewModels;

namespace Sunctum.Domain.Data.DaoFacade
{
    public static class StarFacade
    {
        public static void InsertOrReplace(BookViewModel target)
        {
            var dao = new StarDao();
            dao.InsertOrReplace(new Models.Star() { ID = target.ID, TypeId = 0, Level = target.StarLevel });
        }
    }
}
