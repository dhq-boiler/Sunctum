

using Sunctum.Domain.Data.Dao;

namespace Sunctum.Domain.Models.Managers
{
    public interface IDataAccessManager
    {
        IDaoBuilder AppDao { get; set; }

        IDaoBuilder WorkingDao { get; set; }
        
        IDaoBuilder VcDao { get; set; }
    }
}
