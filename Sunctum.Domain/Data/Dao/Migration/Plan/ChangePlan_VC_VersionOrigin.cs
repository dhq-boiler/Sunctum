

using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using Homura.ORM.Setup;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    public class ChangePlan_VC_VersionOrigin : ChangePlan<VersionOrigin>
    {
        public ChangePlan_VC_VersionOrigin(VersioningMode mode) : base(mode)
        {
        }

        public override IEnumerable<IEntityVersionChangePlan> VersionChangePlanList
        {
            get
            {
                yield return new ChangePlan_VC_VersionControl_VersionOrigin(Mode);
                yield return new ChangePlan_VC_GitHubReleasesLatest_VersionOrigin(Mode);
            }
        }
    }
}
