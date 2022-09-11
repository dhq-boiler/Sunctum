

using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using System.Collections.Generic;

namespace boilersUpdater.Data.Dao.Migration.Plan
{
    public class ChangePlan_VC_VersionOrigin : ChangePlanByVersion<VersionOrigin>
    {
        public override IEnumerable<IEntityVersionChangePlan> VersionChangePlanList
        {
            get
            {
                yield return new ChangePlan_VC_VersionControl_VersionOrigin();
            }
        }
    }
}
