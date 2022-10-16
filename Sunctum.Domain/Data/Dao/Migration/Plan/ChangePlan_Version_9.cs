

using Homura.ORM.Migration;
using Homura.ORM.Setup;
using Sunctum.Domain.Models.Conversion;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    public class ChangePlan_Version_9 : ChangePlan<Version_9>
    {
        public ChangePlan_Version_9(VersioningMode mode) : base(mode)
        {
        }

        public override IEnumerable<IEntityVersionChangePlan> VersionChangePlanList
        {
            get
            {
                yield return new ChangePlan_DoNothing(Mode);
            }
        }
    }
}
