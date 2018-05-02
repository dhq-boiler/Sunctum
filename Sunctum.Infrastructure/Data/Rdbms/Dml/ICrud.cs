

using System.Collections.Generic;
using System.Data;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public interface ICrud
    {
        IEnumerable<IRightValue> Parameters { get; }

        void SetParameters(IDbCommand command);
    }
}
