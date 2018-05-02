

using Sunctum.Infrastructure.Data.Rdbms.Dml;

namespace Sunctum.Infrastructure.Data.Rdbms
{
    public interface IRightValue : ISqlize
    {
        string Name { get; set; }

        object[] Values { get; set; }
    }
}
