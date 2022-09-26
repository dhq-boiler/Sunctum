

using Homura.ORM.Migration;
using Sunctum.Domain.Models.Conversion;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    public class ChangePlan_Version_3 : ChangePlanByVersion<Version_3>
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
