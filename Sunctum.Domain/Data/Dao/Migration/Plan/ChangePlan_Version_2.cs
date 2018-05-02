

using Sunctum.Domain.Models.Conversion;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Migration;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_Version_2 : ChangePlanByVersion<Version_2>
    {
        public override IEnumerable<IEntityVersionChangePlan> VersionChangePlanList
        {
            get
            {
                yield return new ChangePlan_DirectoryNameParser_VersionOrigin();
            }
        }
    }
}
