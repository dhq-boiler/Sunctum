

using Homura.ORM.Migration;
using Homura.ORM.Setup;
using Sunctum.Domain.Models.Conversion;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    public class ChangePlan_Version_8 : ChangePlan<Version_8>
    {
        public ChangePlan_Version_8(VersioningMode mode) : base(mode)
        {
        }

        public override IEnumerable<IEntityVersionChangePlan> VersionChangePlanList
        {
            get
            {
                yield return new ChangePlan_KeyValue_VersionOrigin(Mode);
            }
        }
    }
}
