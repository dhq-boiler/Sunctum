

using Homura.ORM.Migration;
using Sunctum.Domain.Models.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    public class ChangePlan_Version_4 : ChangePlanByVersion<Version_4>
    {
        public override IEnumerable<IEntityVersionChangePlan> VersionChangePlanList
        {
            get
            {
                yield return new ChangePlan_EncryptImage_VersionOrigin();
            }
        }
    }
}
