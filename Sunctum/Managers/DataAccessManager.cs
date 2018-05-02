

using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Models.Managers;

namespace Sunctum.Managers
{
    public class DataAccessManager : IDataAccessManager
    {
        public IDaoBuilder AppDao { get; set; }

        public IDaoBuilder WorkingDao { get; set; }
    }
}
