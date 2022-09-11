

using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Models.Managers;
using Unity;

namespace Sunctum.Managers
{
    public class DataAccessManager : IDataAccessManager
    {
        [Dependency("AppDao")]
        public IDaoBuilder AppDao { get; set; }

        [Dependency("WorkingDao")]
        public IDaoBuilder WorkingDao { get; set; }

        [Dependency("VcDao")]
        public IDaoBuilder VcDao { get; set; }
    }
}
