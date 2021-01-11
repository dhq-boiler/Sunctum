

using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    public class ChangePlan_AppDb_VersionOrigin : ChangePlanByVersion<VersionOrigin>
    {
        public override IEnumerable<IEntityVersionChangePlan> VersionChangePlanList
        {
            get
            {
                yield return new ChangePlan_AppDb_RecentOpenedLibrary_VersionOrigin();
            }
        }
    }
}
