

using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using Homura.ORM.Setup;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    public class ChangePlan_AppDb_VersionOrigin : ChangePlan<VersionOrigin>
    {
        public ChangePlan_AppDb_VersionOrigin(VersioningMode mode) : base(mode)
        {
        }

        public override IEnumerable<IEntityVersionChangePlan> VersionChangePlanList
        {
            get
            {
                yield return new ChangePlan_AppDb_RecentOpenedLibrary_VersionOrigin(Mode);
            }
        }
    }
}
