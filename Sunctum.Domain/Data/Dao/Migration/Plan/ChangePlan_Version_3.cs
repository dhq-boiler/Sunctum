

using Sunctum.Domain.Models.Conversion;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Migration;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_Version_3 : ChangePlanByVersion<Version_3>
    {
        public override IEnumerable<IEntityVersionChangePlan> VersionChangePlanList
        {
            get
            {
                yield return new ChangePlan_BookTag_VersionOrigin();
                yield return new ChangePlan_Star_VersionOrigin();
            }
        }
    }
}
