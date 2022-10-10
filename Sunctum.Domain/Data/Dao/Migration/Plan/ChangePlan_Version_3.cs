

using Homura.ORM.Migration;
using Homura.ORM.Setup;
using Sunctum.Domain.Models.Conversion;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    public class ChangePlanVersion3 : ChangePlan<Version_3>
    {
        public ChangePlanVersion3(VersioningMode mode) : base(mode)
        {
        }

        public override IEnumerable<IEntityVersionChangePlan> VersionChangePlanList
        {
            get
            {
                yield return new ChangePlan_BookTag_VersionOrigin(Mode);
                yield return new ChangePlan_Star_VersionOrigin(Mode);
            }
        }
    }
}
